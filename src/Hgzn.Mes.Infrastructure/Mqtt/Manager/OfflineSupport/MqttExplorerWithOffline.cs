using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// 支持断点续传的MQTT管理器实现
    /// </summary>
    public class MqttExplorerWithOffline : IMqttExplorerWithOffline
    {
        private readonly ILogger<MqttExplorerWithOffline> _logger;
        private readonly ManagedMqttClientOptions _mqttClientOptions;
        private readonly IManagedMqttClient _mqttClient;
        private readonly ISqlSugarClient _client;
        private readonly IMqttOfflineStorage _offlineStorage;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        
        private volatile bool _isConnected = false;
        private readonly SemaphoreSlim _publishSemaphore = new SemaphoreSlim(1, 1);
        private readonly Timer _retryTimer;
        private readonly Timer _cleanupTimer;

        public event EventHandler<bool> ConnectionStatusChanged;

        public MqttExplorerWithOffline(
            ILogger<MqttExplorerWithOffline> logger,
            IConfiguration configuration,
            ISqlSugarClient client,
            IMqttOfflineStorage offlineStorage,
            IConnectionMultiplexer connectionMultiplexer,
            IotMessageHandler messageHandler)
        {
            _logger = logger;
            _client = client;
            _offlineStorage = offlineStorage;
            _connectionMultiplexer = connectionMultiplexer;
            
            var mqtt = configuration.GetConnectionString("Mqtt")!.Split(':');

            _mqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(10))
                .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithTcpServer(mqtt[0], Convert.ToInt32(mqtt[1]))
                    .WithCredentials(mqtt[2], mqtt[3])
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(10))
                    .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
                    .WithWillTopic(TopicBuilder.CreateBuilder()
                        .WithPrefix(TopicType.Sys)
                        .WithDirection(MqttDirection.Up)
                        .WithTag(MqttTag.State)
                        .Build())
                    .WithWillPayload([(byte)MqttState.Will])
                    .WithCleanSession()
                    .Build())
                .Build();
                
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateManagedMqttClient();
            
            // 设置连接事件
            _mqttClient.ConnectedAsync += OnConnectedAsync;
            _mqttClient.DisconnectedAsync += OnDisconnectedAsync;
            _mqttClient.ApplicationMessageReceivedAsync += async (args) =>
            {
                try
                {
                    messageHandler.Initialize(this);
                    await messageHandler.HandleAsync(args.ApplicationMessage);
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case NotSupportedException:
                            break;
                        default:
                            _logger.LogError(ex, "Failed to handle MQTT message");
                            break;
                    }
                }
            };

            // 设置定时器：每30秒尝试重发离线消息
            _retryTimer = new Timer(async _ => await TryResendOfflineMessagesAsync(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            
            // 设置定时器：每小时清理过期消息
            _cleanupTimer = new Timer(async _ => await _offlineStorage.DeleteExpiredMessagesAsync(DateTime.UtcNow.AddDays(-7)), null, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
        }

        private async Task OnConnectedAsync(MqttClientConnectedEventArgs e)
        {
            _isConnected = true;
            _logger.LogInformation("MQTT connected");
            
            ConnectionStatusChanged?.Invoke(this, true);
            
            // 连接成功后发布连接状态
            await PublishWithOfflineSupportAsync(TopicBuilder.CreateBuilder()
                .WithPrefix(TopicType.Sys)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.State)
                .Build(),
                [(byte)MqttState.Connected], priority: 1); // 高优先级
            
            // 尝试重发离线消息
            _ = Task.Run(async () => await TryResendOfflineMessagesAsync());
        }

        private async Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs e)
        {
            _isConnected = false;
            _logger.LogWarning("MQTT disconnected");
            
            ConnectionStatusChanged?.Invoke(this, false);
        }

        public async Task StartAsync()
        {
            var topics = await GetSubscribeTopicsAsync();
            _logger.LogInformation($"Starting MQTT client, subscribing to topics: {string.Join("; ", topics.Select(t => t.Topic))}");
            await _mqttClient.SubscribeAsync(topics);
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        public async Task StopAsync()
        {
            _logger.LogInformation("Stopping MQTT client");
            
            await PublishWithOfflineSupportAsync(TopicBuilder.CreateBuilder()
                .WithPrefix(TopicType.Sys)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.State)
                .Build(),
                [(byte)MqttState.Stop], priority: 1);
                
            _retryTimer?.Dispose();
            _cleanupTimer?.Dispose();
            await _mqttClient.StopAsync();
        }

        public async Task RestartAsync()
        {
            _logger.LogInformation("Restarting MQTT client");
            
            await PublishWithOfflineSupportAsync(TopicBuilder.CreateBuilder()
                .WithPrefix(TopicType.Sys)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.State)
                .Build(),
                [(byte)MqttState.Restart], priority: 1);
                
            await _mqttClient.StopAsync();
            var topics = await GetSubscribeTopicsAsync();
            await _mqttClient.SubscribeAsync(topics);
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        public async Task PublishAsync(string topic, byte[] payload)
        {
            await PublishWithOfflineSupportAsync(topic, payload);
        }

        public async Task PublishWithOfflineSupportAsync(string topic, byte[] payload, int priority = 0, int maxRetryCount = 3, bool storeOfflineIfDisconnected = true)
        {
            await _publishSemaphore.WaitAsync();
            try
            {
                if (_isConnected && _mqttClient.IsConnected)
                {
                    // 直接发送
                    await PublishMessageAsync(topic, payload);
                }
                else if (storeOfflineIfDisconnected)
                {
                    // 存储到离线队列
                    var offlineMessage = new MqttOfflineMessage(topic, payload, priority, maxRetryCount);
                    await _offlineStorage.AddOfflineMessageAsync(offlineMessage);
                    _logger.LogDebug($"Stored message to offline queue: {offlineMessage.Id}");
                }
                else
                {
                    _logger.LogWarning($"MQTT not connected and offline storage disabled, dropping message for topic: {topic}");
                }
            }
            finally
            {
                _publishSemaphore.Release();
            }
        }

        private async Task PublishMessageAsync(string topic, byte[] payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .Build();
            await _mqttClient.EnqueueAsync(message);
        }

        public async Task<int> ResendOfflineMessagesAsync()
        {
            if (!_isConnected || !_mqttClient.IsConnected)
            {
                return 0;
            }

            var pendingMessages = await _offlineStorage.GetPendingMessagesAsync(50); // 每次最多处理50条消息
            var resentCount = 0;

            foreach (var message in pendingMessages)
            {
                try
                {
                    await PublishMessageAsync(message.Topic, message.Payload);
                    await _offlineStorage.MarkMessageAsSentAsync(message.Id);
                    resentCount++;
                    
                    _logger.LogDebug($"Resent offline message: {message.Id}");
                    
                    // 避免发送过快
                    await Task.Delay(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to resend offline message: {message.Id}");
                    await _offlineStorage.IncrementRetryCountAsync(message.Id);
                }
            }

            if (resentCount > 0)
            {
                _logger.LogInformation($"Resent {resentCount} offline messages");
            }

            return resentCount;
        }

        private async Task TryResendOfflineMessagesAsync()
        {
            try
            {
                await ResendOfflineMessagesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resend offline messages");
            }
        }

        public async Task CleanupExpiredMessagesAsync(DateTime expiredBefore)
        {
            await _offlineStorage.DeleteExpiredMessagesAsync(expiredBefore);
        }

        public async Task<(int pendingCount, int totalCount)> GetOfflineMessageStatsAsync()
        {
            var pendingMessages = await _offlineStorage.GetPendingMessagesAsync(1000);
            return (pendingMessages.Count, pendingMessages.Count);
        }

        public async Task SubscribeAsync(string topic)
        {
            await _mqttClient.SubscribeAsync(topic);
        }

        public async Task UnSubscribeAsync(string topic)
        {
            await _mqttClient.UnsubscribeAsync(topic);
        }

        public async Task<ICollection<MqttTopicFilter>> GetSubscribeTopicsAsync()
        {
            var types = await _client.Queryable<EquipType>()
                .Where(t => !string.IsNullOrEmpty(t.ProtocolEnum))
                .Select(dt => dt.ProtocolEnum)
                .ToArrayAsync();
                
            return types
                .Select(type => $"{TopicType.Iot:F}/+/{IotTopic.EquipTypeName}/{type}/{IotTopic.ConnUriName}/+/+".ToLower())
                .Select(topic => new MqttTopicFilterBuilder().WithTopic(topic)
                    .WithExactlyOnceQoS()
                    .Build()).ToArray();
        }

        public Task<bool> IsConnectedAsync()
        {
            return Task.FromResult(_isConnected && _mqttClient.IsConnected);
        }

        public void Dispose()
        {
            _retryTimer?.Dispose();
            _cleanupTimer?.Dispose();
            _publishSemaphore?.Dispose();
            _mqttClient?.Dispose();
        }
    }
}
