using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Services.Audit.IService;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hgzn.Mes.WebApi.Controllers.Audit;

/// <summary>
/// 增量备份控制器
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class IncrementalBackupController : ControllerBase
{
    private readonly IIncrementalBackupService _backupService;
    private readonly ILogger<IncrementalBackupController> _logger;

    public IncrementalBackupController(
        IIncrementalBackupService backupService,
        ILogger<IncrementalBackupController> logger)
    {
        _backupService = backupService;
        _logger = logger;
    }

    /// <summary>
    /// 增量备份导出接口
    /// 其他电脑调用此接口获取增量更新的数据文件
    /// </summary>
    /// <param name="request">增量备份请求</param>
    /// <returns>备份文件</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("export")]
    [AllowAnonymous] // 如果需要认证，可以改为 [Authorize]
    public async Task<IActionResult> ExportIncrementalBackupAsync([FromBody] IncrementalBackupRequestDto request)
    {
        try
        {
            _logger.LogInformation($"收到增量备份导出请求 - 客户端ID: {request.ClientId ?? "null"}, 压缩: {request.Compress}, 数据库名: {request.DatabaseName ?? "null"}, 表名: {request.TableName ?? "null"}, 上次导出时间: {request.LastExportTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "null"}");

            if (string.IsNullOrEmpty(request.ClientId))
            {
                _logger.LogWarning("增量备份导出失败 - ClientId为空");
                return BadRequest(new { message = "ClientId不能为空" });
            }

            _logger.LogInformation($"开始调用备份服务导出接口 - 客户端ID: {request.ClientId}");
            var exportData = await _backupService.ExportIncrementalBackupAsync(request);
            _logger.LogInformation($"备份服务导出完成 - 客户端ID: {request.ClientId}, 导出数据大小: {exportData.Length} 字节");

            // JSON格式文件名和ContentType
            string fileName = $"增量备份_{request.ClientId}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string contentType = "application/json";

            // 如果压缩，修改文件名和ContentType
            if (request.Compress)
            {
                fileName += ".gz";
                contentType = "application/gzip";
            }

            _logger.LogInformation($"准备返回备份文件 - 文件名: {fileName}, ContentType: {contentType}, 文件大小: {exportData.Length} 字节");
            return File(exportData, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"增量备份导出失败 - 客户端ID: {request?.ClientId ?? "null"}, 压缩: {request?.Compress}");
            return BadRequest(new { message = $"增量备份导出失败: {ex.Message}" });
        }
    }

    /// <summary>
    /// 获取客户端的备份状态（上次导出时间、记录数等）
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <returns>备份状态信息</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("status/{clientId}")]
    [AllowAnonymous]
    public async Task<ResponseWrapper<IncrementalBackupStatusDto>> GetBackupStatusAsync(string clientId)
    {
        try
        {
            _logger.LogInformation($"收到查询备份状态请求 - 客户端ID: {clientId ?? "null"}");
            
            var status = await _backupService.GetBackupStatusAsync(clientId);
            
            if (status != null)
            {
                _logger.LogInformation($"查询备份状态成功 - 客户端ID: {status.ClientId}, 上次导出时间: {status.LastExportTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "无"}, 是否有导出历史: {status.HasExportHistory}, 上次导出记录数: {status.LastExportRecordCount ?? 0}");
            }
            else
            {
                _logger.LogWarning($"查询备份状态返回空数据 - 客户端ID: {clientId ?? "null"}");
            }
            
            return status.Wrap();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"查询备份状态时发生异常 - 客户端ID: {clientId ?? "null"}");
            throw;
        }
    }

    /// <summary>
    /// 获取增量备份元数据信息（不导出文件，只返回统计信息）
    /// </summary>
    /// <param name="request">增量备份请求</param>
    /// <returns>备份元数据信息</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("metadata")]
    [AllowAnonymous]
    public async Task<ResponseWrapper<IncrementalBackupResultDto>> GetBackupMetadataAsync(
        [FromBody] IncrementalBackupRequestDto request)
    {
        try
        {
            _logger.LogInformation($"收到获取备份元数据请求 - 客户端ID: {request.ClientId ?? "null"}, 压缩: {request.Compress}, 数据库名: {request.DatabaseName ?? "null"}, 表名: {request.TableName ?? "null"}");
            
            var metadata = await _backupService.GetBackupMetadataAsync(request);
            
            if (metadata != null)
            {
                _logger.LogInformation($"获取备份元数据成功 - 客户端ID: {request.ClientId}, 记录数: {metadata.RecordCount}, 总记录数: {metadata.TotalCount}");
            }
            else
            {
                _logger.LogWarning($"获取备份元数据返回空数据 - 客户端ID: {request.ClientId ?? "null"}");
            }
            
            return metadata.Wrap();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"获取备份元数据时发生异常 - 客户端ID: {request?.ClientId ?? "null"}");
            throw;
        }
    }

    /// <summary>
    /// 导入增量备份文件并更新数据库（JSON格式）
    /// </summary>
    /// <param name="file">备份文件（JSON格式）</param>
    /// <param name="compress">是否压缩</param>
    /// <param name="clientId">客户端ID（可选）</param>
    /// <returns>导入结果</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Route("import")]
    [AllowAnonymous]
    [DisableRequestSizeLimit] // 禁用请求大小限制，允许大文件上传
    [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue, ValueLengthLimit = int.MaxValue)]
    public async Task<ResponseWrapper<IncrementalBackupImportResultDto>> ImportIncrementalBackupAsync(
        IFormFile file,
        [FromForm] bool compress = false,
        [FromForm] string? clientId = null)
    {
        try
        {
            _logger.LogInformation($"收到导入增量备份文件请求 - 文件名: {file?.FileName ?? "null"}, 文件大小: {file?.Length ?? 0} 字节, 压缩: {compress}, 客户端ID: {clientId ?? "null"}");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("导入增量备份文件失败 - 文件为空或文件大小为0");
                return new ResponseWrapper<IncrementalBackupImportResultDto>
                {
                    Info = "文件不能为空",
                    Status = StatusCodes.Status400BadRequest
                };
            }

            // 读取文件数据
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                _logger.LogInformation($"开始读取文件数据 - 文件名: {file.FileName}, 文件大小: {file.Length} 字节");
                await file.CopyToAsync(memoryStream);
                fileData = memoryStream.ToArray();
                _logger.LogInformation($"文件数据读取完成 - 实际读取大小: {fileData.Length} 字节");
            }

            // 构建导入请求
            var request = new IncrementalBackupImportRequestDto
            {
                FileData = fileData,
                Compress = compress,
                ClientId = clientId
            };

            _logger.LogInformation($"准备调用备份服务导入接口 - 压缩: {request.Compress}, 客户端ID: {request.ClientId ?? "null"}, 数据大小: {request.FileData.Length} 字节");

            // 执行导入
            var result = await _backupService.ImportIncrementalBackupAsync(request);
            
            if (result.Success)
            {
                _logger.LogInformation($"增量备份导入成功 - 新增/更新记录数: {result.InsertUpdateCount}, 总记录数: {result.TotalCount}, 导入时间: {result.ImportTime}");
                return result.Wrap();
            }
            else
            {
                _logger.LogError($"增量备份导入失败 - 错误信息: {result.ErrorMessage ?? "未知错误"}");
                return new ResponseWrapper<IncrementalBackupImportResultDto>
                {
                    Info = result.ErrorMessage ?? "导入失败",
                    Data = result,
                    Status = StatusCodes.Status400BadRequest
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"导入增量备份文件时发生异常 - 文件名: {file?.FileName ?? "null"}, 压缩: {compress}, 客户端ID: {clientId ?? "null"}");
            return new ResponseWrapper<IncrementalBackupImportResultDto>
            {
                Info = $"导入增量备份失败: {ex.Message}",
                Status = StatusCodes.Status400BadRequest
            };
        }
    }
}
