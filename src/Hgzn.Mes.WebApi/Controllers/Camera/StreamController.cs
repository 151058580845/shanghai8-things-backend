using Hgzn.Mes.Application.Main.Services.Camera.IService;
using Hgzn.Mes.Domain.Entities.Camera;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Camera;

[Route("api/[controller]")]
[ApiController]
public class StreamController : ControllerBase
{
    readonly IStreamService _streamingService;
    readonly ICameraService _cameraService;
    readonly ILogger<StreamController> _logger;

    public StreamController(IStreamService streamingService, ICameraService cameraService, ILogger<StreamController> logger)
    {
        _streamingService = streamingService;
        _cameraService = cameraService;
        _logger = logger;
    }

    // 启动推流
    [HttpGet]
    [Route("start-stream/{cameraId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public async Task<IActionResult> StartStreaming(string cameraId)
    {
        try
        {
            var camera = _cameraService.GetCamera(cameraId);
            if (camera == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "摄像头不存在"
                }.Wrap());
            }

            var result = await _streamingService.StartStreamingAsync(cameraId, camera);

            if (result)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "推流启动成功",
                    Data = new { streamUrl = _streamingService.GetStreamUrl(cameraId) }
                }.Wrap());
            }
            else
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "推流启动失败"
                }.Wrap());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"启动推流失败: {cameraId}");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"启动推流失败: {ex.Message}"
            }.Wrap());
        }
    }

    // 获取推流URL
    [HttpGet]
    [Route("url/{cameraId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public IActionResult GetStreamUrl(string cameraId)
    {
        if (_streamingService.IsStreaming(cameraId))
        {
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { streamUrl = _streamingService.GetStreamUrl(cameraId) }
            }.Wrap());
        }
        else
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "推流未启动"
            }.Wrap());
        }
    }
}
