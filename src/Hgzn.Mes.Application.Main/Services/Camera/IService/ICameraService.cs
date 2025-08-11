using Hgzn.Mes.Domain.Entities.Camera;

namespace Hgzn.Mes.Application.Main.Services.Camera.IService;

public interface ICameraService
{
    /// <summary>
    /// 获取所有摄像头信息
    /// </summary>
    /// <returns></returns>
    List<CameraInfo> GetAllCameras();
    /// <summary>
    /// 根据"ip_port"格式的id获取摄像头信息
    /// </summary>
    /// <param name="cameraId"></param>
    /// <returns></returns>
    CameraConfig? GetCamera(string cameraId);
    /// <summary>
    /// 根据"ip_port"格式的id获取摄像头信息, 没有就添加
    /// </summary>
    /// <param name="cameraId"></param>
    /// <returns></returns>
    CameraConfig? GetCamera(string cameraId, CameraConfig cameraConfig);
    /// <summary>
    /// 根据id获取摄像头连接状态
    /// </summary>
    bool IsCameraConnected(string cameraId);
    /// <summary>
    /// 根据id获取设置摄像头连接状态
    /// </summary>
    void SetCameraConnected(string cameraId, bool connected);
    /// <summary>
    /// 添加摄像头
    /// </summary>
    /// <param name="config"></param>
    void AddCamera(CameraConfig config);
    /// <summary>
    /// 移除摄像头
    /// </summary>
    void RemoveCamera(string cameraId);
}
