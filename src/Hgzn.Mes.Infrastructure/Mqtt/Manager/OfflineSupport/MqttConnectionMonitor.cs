using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT连接状态监控实现
    /// </summary>
    public class MqttConnectionMonitor : IMqttConnectionMonitor, IDisposable
    {
        private readonly ILogger<MqttConnectionMonitor> _logger;
        private readonly IMqttExplorerWithOffline _mqttExplorer;
        private readonly Timer _healthCheckTimer;
        
        private volatile bool _isMonitoring = false;
        private volatile bool _lastKnownStatus = false;
        private DateTime _lastConnectedTime = DateTime.MinValue;
        private DateTime _lastDisconnectedTime = DateTime.MinValue;
        private int _totalReconnectCount = 0;
        private int _failedReconnectCount = 0;
        private TimeSpan _totalConnectedDuration = TimeSpan.Zero;
        private DateTime _monitoringStartTime = DateTime.UtcNow;
        private DateTime _lastHealthCheckTime = DateTime.UtcNow;

        public event EventHandler<MqttConnectionStatusEventArgs> ConnectionStatusChanged;

        public MqttConnectionMonitor(ILogger<MqttConnectionMonitor> logger, IMqttExplorerWithOffline mqttExplorer)
        {
            _logger = logger;
            _mqttExplorer = mqttExplorer;
            
            // 每30秒检查一次连接状态
            _healthCheckTimer = new Timer(HealthCheckCallback, null, Timeout.Infinite, 30000);
            
            // 订阅MQTT连接状态变化事件
            _mqttExplorer.ConnectionStatusChanged += OnMqttConnectionStatusChanged;
        }

        private void OnMqttConnectionStatusChanged(object sender, bool isConnected)
        {
            var timestamp = DateTime.UtcNow;
            
            // 记录连接/断开时间
            if (isConnected && !_lastKnownStatus)
            {
                _lastConnectedTime = timestamp;
                if (_lastDisconnectedTime != DateTime.MinValue)
                {
                    _totalReconnectCount++;
                }
                _logger.LogInformation("MQTT connection established");
            }
            else if (!isConnected && _lastKnownStatus)
            {
                _lastDisconnectedTime = timestamp;
                if (_lastConnectedTime != DateTime.MinValue)
                {
                    _totalConnectedDuration += timestamp - _lastConnectedTime;
                }
                _logger.LogWarning("MQTT connection lost");
            }
            
            _lastKnownStatus = isConnected;
            
            // 触发状态变化事件
            var eventArgs = new MqttConnectionStatusEventArgs(isConnected, timestamp);
            ConnectionStatusChanged?.Invoke(this, eventArgs);
        }

        private async void HealthCheckCallback(object state)
        {
            if (!_isMonitoring)
                return;
                
            try
            {
                var isConnected = await _mqttExplorer.IsConnectedAsync();
                
                // 如果状态发生变化，触发事件
                if (isConnected != _lastKnownStatus)
                {
                    OnMqttConnectionStatusChanged(this, isConnected);
                }
                
                // 记录健康检查时间
                _lastHealthCheckTime = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                _failedReconnectCount++;
            }
        }

        public async Task<bool> IsConnectedAsync()
        {
            return await _mqttExplorer.IsConnectedAsync();
        }

        public async Task<MqttConnectionStats> GetConnectionStatsAsync()
        {
            var isConnected = await _mqttExplorer.IsConnectedAsync();
            var now = DateTime.UtcNow;
            
            // 计算平均连接时长
            var averageConnectedDuration = _totalReconnectCount > 0 
                ? TimeSpan.FromTicks(_totalConnectedDuration.Ticks / _totalReconnectCount)
                : TimeSpan.Zero;
            
            // 计算健康状态（连接状态 + 最近健康检查时间）
            var isHealthy = isConnected && (now - _lastHealthCheckTime).TotalMinutes < 2;
            
            return new MqttConnectionStats
            {
                IsConnected = isConnected,
                LastConnectedTime = _lastConnectedTime,
                LastDisconnectedTime = _lastDisconnectedTime,
                TotalReconnectCount = _totalReconnectCount,
                FailedReconnectCount = _failedReconnectCount,
                TotalConnectedDuration = _totalConnectedDuration,
                AverageConnectedDuration = averageConnectedDuration,
                LastHealthCheckTime = _lastHealthCheckTime,
                IsHealthy = isHealthy
            };
        }

        public async Task StartMonitoringAsync()
        {
            if (_isMonitoring)
                return;
                
            _isMonitoring = true;
            _monitoringStartTime = DateTime.UtcNow;
            
            // 启动健康检查定时器
            _healthCheckTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(30));
            
            // 立即检查一次连接状态
            var isConnected = await _mqttExplorer.IsConnectedAsync();
            OnMqttConnectionStatusChanged(this, isConnected);
            
            _logger.LogInformation("MQTT connection monitoring started");
        }

        public async Task StopMonitoringAsync()
        {
            if (!_isMonitoring)
                return;
                
            _isMonitoring = false;
            
            // 停止健康检查定时器
            _healthCheckTimer.Change(Timeout.Infinite, Timeout.Infinite);
            
            _logger.LogInformation("MQTT connection monitoring stopped");
            
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _healthCheckTimer?.Dispose();
            
            if (_mqttExplorer is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
