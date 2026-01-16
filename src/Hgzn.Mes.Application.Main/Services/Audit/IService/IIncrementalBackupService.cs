using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Services;

namespace Hgzn.Mes.Application.Main.Services.Audit.IService;

/// <summary>
/// 增量备份服务接口
/// </summary>
public interface IIncrementalBackupService : IBaseService
{
    /// <summary>
    /// 增量备份导出
    /// </summary>
    Task<byte[]> ExportIncrementalBackupAsync(IncrementalBackupRequestDto request);
    
    /// <summary>
    /// 获取增量备份状态
    /// </summary>
    Task<IncrementalBackupStatusDto> GetBackupStatusAsync(string clientId);
    
    /// <summary>
    /// 获取增量备份元数据信息
    /// </summary>
    Task<IncrementalBackupResultDto> GetBackupMetadataAsync(IncrementalBackupRequestDto request);
    
    /// <summary>
    /// 导入增量备份文件并更新数据库
    /// </summary>
    Task<IncrementalBackupImportResultDto> ImportIncrementalBackupAsync(IncrementalBackupImportRequestDto request);
}
