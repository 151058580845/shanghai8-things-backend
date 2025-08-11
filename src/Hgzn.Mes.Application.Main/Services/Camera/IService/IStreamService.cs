using Hgzn.Mes.Domain.Entities.Camera;

namespace Hgzn.Mes.Application.Main.Services.Camera.IService;

public interface IStreamService
{
    Task<bool> StartStreamingAsync(string cameraId, CameraConfig config);
    Task StopStreamingAsync(string cameraId);
    bool IsStreaming(string cameraId);
    string GetStreamUrl(string cameraId);
}
