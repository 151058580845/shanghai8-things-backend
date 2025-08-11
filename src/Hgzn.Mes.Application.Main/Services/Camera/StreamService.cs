using System.Diagnostics;
using Hgzn.Mes.Application.Main.Services.Camera.IService;
using Hgzn.Mes.Domain.Entities.Camera;
using Microsoft.Extensions.Logging;

namespace Hgzn.Mes.Application.Main.Services.Camera;

public class StreamService : IStreamService
{
    readonly static Dictionary<string, StreamProcess> _streams = new();
    readonly static string _streamServerUrl = "http://localhost:8080/live"; // nginx-http-flv-module服务器
    readonly ILogger<StreamService> _logger;
    readonly static object _lockObject = new();

    public StreamService(ILogger<StreamService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> StartStreamingAsync(string cameraId, CameraConfig config)
    {
        
        if (_streams.ContainsKey(cameraId))
        {
            return true;
        }

        try
        {
            var streamKey = cameraId;
            var rtspUrl = GetRtspUrl(config);

            // FFmpeg推流命令
            var ffmpegArgs = new[]
            {
                "-rtsp_transport", "tcp",
                "-i", rtspUrl,
                "-c:v", "libx264",
                "-c:a", "aac",
                "-f", "flv",
                $"rtmp://localhost/live/{streamKey}"
            };

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = string.Join(" ", ffmpegArgs.Select(arg => $"\"{arg}\"")),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    _logger.LogDebug($"FFmpeg推流 [{cameraId}]: {e.Data}");
                }
            };

            process.Start();
            process.BeginErrorReadLine();

            lock (_lockObject)
            {
                _streams[cameraId] = new StreamProcess
                {
                    Process = process,
                    StreamKey = streamKey,
                    StartTime = DateTime.UtcNow
                };
            }

            _logger.LogInformation($"开始推流: {cameraId} -> {streamKey}");
            await Task.CompletedTask;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"启动推流失败: {cameraId}");
            return false;
        }
    }

    public async Task StopStreamingAsync(string cameraId)
    {
        StreamProcess? streamProcess = null;
        lock (_lockObject)
        {
            if (_streams.TryGetValue(cameraId, out var process))
            {
                streamProcess = process;
                _streams.Remove(cameraId);
            }
        }

        if (streamProcess?.Process != null)
        {
            try
            {
                if (!streamProcess.Process.HasExited)
                {
                    streamProcess.Process.Kill();
                    await streamProcess.Process.WaitForExitAsync();
                }
                _logger.LogInformation($"停止推流: {cameraId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"停止推流失败: {cameraId}");
            }
        }
    }

    public bool IsStreaming(string cameraId)
    {
        lock (_lockObject)
        {
            return _streams.ContainsKey(cameraId) &&
                   _streams[cameraId].Process?.HasExited == false;
        }
    }

    public string GetStreamUrl(string cameraId)
    {
        return $"{_streamServerUrl}/{cameraId}.flv";
    }

    private string GetRtspUrl(CameraConfig config)
    {
        return $"rtsp://{config.Username}:{config.Password}@{config.Ip}:554/Streaming/Channels/{config.Channel}01";
    }
}

public class StreamProcess
{
    public Process? Process { get; set; }
    public string StreamKey { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
}
