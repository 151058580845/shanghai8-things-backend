using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT断点续传功能测试服务
    /// </summary>
    public class MqttOfflineTestService
    {
        private readonly ILogger<MqttOfflineTestService> _logger;
        private readonly IMqttExplorerWithOffline _mqttExplorer;
        private readonly IMqttConnectionMonitor _connectionMonitor;

        public MqttOfflineTestService(
            ILogger<MqttOfflineTestService> logger,
            IMqttExplorerWithOffline mqttExplorer,
            IMqttConnectionMonitor connectionMonitor)
        {
            _logger = logger;
            _mqttExplorer = mqttExplorer;
            _connectionMonitor = connectionMonitor;
        }

        /// <summary>
        /// 测试断点续传功能
        /// </summary>
        public async Task TestOfflineFunctionalityAsync()
        {
            _logger.LogInformation("开始测试MQTT断点续传功能...");

            // 1. 测试连接状态
            await TestConnectionStatusAsync();

            // 2. 测试不同优先级的消息发送
            await TestPriorityMessagesAsync();

            // 3. 测试离线消息统计
            await TestOfflineMessageStatsAsync();

            // 4. 测试连接监控
            await TestConnectionMonitoringAsync();

            _logger.LogInformation("MQTT断点续传功能测试完成");
        }

        private async Task TestConnectionStatusAsync()
        {
            var isConnected = await _mqttExplorer.IsConnectedAsync();
            _logger.LogInformation($"当前MQTT连接状态: {(isConnected ? "已连接" : "未连接")}");
        }

        private async Task TestPriorityMessagesAsync()
        {
            _logger.LogInformation("测试不同优先级的消息发送...");

            var messages = new[]
            {
                new { Topic = "test/low-priority", Message = "低优先级消息", Priority = 3 },
                new { Topic = "test/normal-priority", Message = "普通优先级消息", Priority = 2 },
                new { Topic = "test/high-priority", Message = "高优先级消息", Priority = 1 },
                new { Topic = "test/critical-priority", Message = "关键优先级消息", Priority = 0 }
            };

            foreach (var msg in messages)
            {
                var payload = System.Text.Encoding.UTF8.GetBytes($"{msg.Message} - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                await _mqttExplorer.PublishWithOfflineSupportAsync(
                    msg.Topic, 
                    payload, 
                    priority: msg.Priority,
                    maxRetryCount: 3);
                
                _logger.LogInformation($"发送消息: {msg.Message} (优先级: {msg.Priority})");
                await Task.Delay(100); // 避免发送过快
            }
        }

        private async Task TestOfflineMessageStatsAsync()
        {
            _logger.LogInformation("获取离线消息统计信息...");
            
            var stats = await _mqttExplorer.GetOfflineMessageStatsAsync();
            _logger.LogInformation($"离线消息统计 - 待发送: {stats.pendingCount}, 总数: {stats.totalCount}");
        }

        private async Task TestConnectionMonitoringAsync()
        {
            _logger.LogInformation("测试连接监控功能...");
            
            var stats = await _connectionMonitor.GetConnectionStatsAsync();
            _logger.LogInformation($"连接统计信息:");
            _logger.LogInformation($"  - 当前状态: {(stats.IsConnected ? "已连接" : "未连接")}");
            _logger.LogInformation($"  - 最后连接时间: {stats.LastConnectedTime:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation($"  - 最后断开时间: {stats.LastDisconnectedTime:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation($"  - 总重连次数: {stats.TotalReconnectCount}");
            _logger.LogInformation($"  - 失败重连次数: {stats.FailedReconnectCount}");
            _logger.LogInformation($"  - 总连接时长: {stats.TotalConnectedDuration}");
            _logger.LogInformation($"  - 平均连接时长: {stats.AverageConnectedDuration}");
            _logger.LogInformation($"  - 最后健康检查时间: {stats.LastHealthCheckTime:yyyy-MM-dd HH:mm:ss}");
            _logger.LogInformation($"  - 健康状态: {(stats.IsHealthy ? "健康" : "异常")}");
        }

        /// <summary>
        /// 模拟网络断开场景测试
        /// </summary>
        public async Task SimulateNetworkDisconnectionTestAsync()
        {
            _logger.LogInformation("开始模拟网络断开场景测试...");

            // 1. 发送一些消息
            for (int i = 0; i < 5; i++)
            {
                var topic = $"test/simulation/message-{i}";
                var payload = System.Text.Encoding.UTF8.GetBytes($"模拟消息 {i} - {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
                await _mqttExplorer.PublishWithOfflineSupportAsync(topic, payload, priority: 1);
                _logger.LogInformation($"发送模拟消息 {i}");
                await Task.Delay(200);
            }

            // 2. 检查离线消息数量
            var stats = await _mqttExplorer.GetOfflineMessageStatsAsync();
            _logger.LogInformation($"网络断开前离线消息数量: {stats.pendingCount}");

            // 3. 等待一段时间（模拟网络断开）
            _logger.LogInformation("等待30秒（模拟网络断开）...");
            await Task.Delay(30000);

            // 4. 再次检查离线消息数量
            var statsAfter = await _mqttExplorer.GetOfflineMessageStatsAsync();
            _logger.LogInformation($"网络断开后离线消息数量: {statsAfter.pendingCount}");

            // 5. 尝试重发离线消息
            var resentCount = await _mqttExplorer.ResendOfflineMessagesAsync();
            _logger.LogInformation($"重发了 {resentCount} 条离线消息");

            _logger.LogInformation("网络断开场景测试完成");
        }
    }
}
