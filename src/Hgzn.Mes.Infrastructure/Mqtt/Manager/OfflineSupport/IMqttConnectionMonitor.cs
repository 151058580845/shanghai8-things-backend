using System;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT连接状态监控接口
    /// </summary>
    public interface IMqttConnectionMonitor
    {
        /// <summary>
        /// 连接状态变化事件
        /// </summary>
        event EventHandler<MqttConnectionStatusEventArgs> ConnectionStatusChanged;

        /// <summary>
        /// 获取当前连接状态
        /// </summary>
        /// <returns></returns>
        Task<bool> IsConnectedAsync();

        /// <summary>
        /// 获取连接统计信息
        /// </summary>
        /// <returns></returns>
        Task<MqttConnectionStats> GetConnectionStatsAsync();

        /// <summary>
        /// 开始监控
        /// </summary>
        Task StartMonitoringAsync();

        /// <summary>
        /// 停止监控
        /// </summary>
        Task StopMonitoringAsync();
    }

    /// <summary>
    /// MQTT连接状态事件参数
    /// </summary>
    public class MqttConnectionStatusEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Reason { get; set; }
        public Exception? Exception { get; set; }

        public MqttConnectionStatusEventArgs(bool isConnected, DateTime timestamp, string? reason = null, Exception? exception = null)
        {
            IsConnected = isConnected;
            Timestamp = timestamp;
            Reason = reason;
            Exception = exception;
        }
    }

    /// <summary>
    /// MQTT连接统计信息
    /// </summary>
    public class MqttConnectionStats
    {
        public bool IsConnected { get; set; }
        public DateTime LastConnectedTime { get; set; }
        public DateTime LastDisconnectedTime { get; set; }
        public int TotalReconnectCount { get; set; }
        public int FailedReconnectCount { get; set; }
        public TimeSpan TotalConnectedDuration { get; set; }
        public TimeSpan AverageConnectedDuration { get; set; }
        public DateTime LastHealthCheckTime { get; set; }
        public bool IsHealthy { get; set; }
    }
}
