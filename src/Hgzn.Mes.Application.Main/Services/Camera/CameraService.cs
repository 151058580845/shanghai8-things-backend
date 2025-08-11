using Hgzn.Mes.Application.Main.Services.Camera.IService;
using Hgzn.Mes.Domain.Entities.Camera;

namespace Hgzn.Mes.Application.Main.Services.Camera;

public class CameraService : ICameraService
{
    readonly static Dictionary<string, CameraConfig> _cameras = [];
    readonly static Dictionary<string, bool> _connectionStatus = [];
    readonly static object _lockObject = new();

    public void InitializeCameras(List<CameraConfig> configs)
    {
        foreach (var config in configs)
        {
            _cameras[$"{config.Ip}_{config.Port}"] = config;
            _connectionStatus[$"{config.Ip}_{config.Port}"] = false;
        }
    }

    public List<CameraInfo> GetAllCameras() => [.. _cameras.Select(kvp => new CameraInfo
        {
            Id = kvp.Key,
            Name = kvp.Key,
            IsConnected = _connectionStatus.GetValueOrDefault(kvp.Key, false)
        })];

    public CameraConfig? GetCamera(string cameraId) => _cameras.GetValueOrDefault(cameraId);

    public CameraConfig? GetCamera(string cameraId, CameraConfig cameraConfig) 
    {
        var camera = _cameras.GetValueOrDefault(cameraId);
        if (camera != null)
        {
            AddCamera(cameraConfig);
        }
        return cameraConfig;
    }

    public bool IsCameraConnected(string cameraId) => _connectionStatus.GetValueOrDefault(cameraId, false);

    public void SetCameraConnected(string cameraId, bool connected)
    {
        _connectionStatus[cameraId] = connected;
    }

    public void AddCamera(CameraConfig config)
    {
        lock (_lockObject)
        {
            if (!_cameras.ContainsKey($"{config.Ip}_{config.Port}"))
            {
                _cameras[$"{config.Ip}_{config.Port}"] = config;
            }
            if (!_connectionStatus.ContainsKey($"{config.Ip}_{config.Port}"))
            {
                _connectionStatus[$"{config.Ip}_{config.Port}"] = false;
            }
        }
    }

    public void RemoveCamera(string cameraId)
    {
        lock (_lockObject)
        {
            _cameras.Remove(cameraId);
            _connectionStatus.Remove(cameraId);
        }
    }
}
