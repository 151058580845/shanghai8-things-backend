using Hgzn.Mes.Application.Main.Services.Camera;
using Hgzn.Mes.Application.Main.Services.Camera.IService;
using Hgzn.Mes.Domain.Entities.Camera;
using Hgzn.Mes.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hgzn.Mes.WebApi.Controllers.Camera;

[Route("api/[controller]")]
[ApiController]
public class CamerasController : ControllerBase
{
    private readonly ICameraService _cameraService;
    private readonly IStreamService _streamService;
    private readonly ILogger<CamerasController> _logger;

    public CamerasController(ICameraService cameraService, IStreamService streamService, ILogger<CamerasController> logger)
    {
        _cameraService = cameraService;
        _streamService = streamService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public IActionResult GetAllCameras()
    {
        try
        {
            var cameras = _cameraService.GetAllCameras();
            return Ok(new ApiResponse<List<CameraInfo>>
            {
                Success = true,
                Data = cameras
            }.Wrap());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"获取摄像头列表失败: {ex.Message}"
            }.Wrap());
        }
    }

    [HttpGet]
    [Route("{cameraId}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public IActionResult GetCameraStatus(string cameraId)
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
                });
            }

            var status = new
            {
                Id = cameraId,
                Name = camera.Name,
                IsConnected = _cameraService.IsCameraConnected(cameraId)
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = status
            }.Wrap());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"获取摄像头状态失败: {ex.Message}"
            }.Wrap());
        }
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
    public IActionResult AddCamera(CameraConfig config)
    {
        try
        {
            _cameraService.AddCamera(config);
            return Ok(new ApiResponse<CameraInfo>
            {
                Success = true,
                Data = new CameraInfo()
                {
                    Id = config.Ip + "_" + config.Port,
                    Name = config.Ip + "_" + config.Port
                },
                Message = "添加摄像头成功"
            }.Wrap());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"添加摄像头失败: {ex.Message}"
            }.Wrap());
        }
    }
}
