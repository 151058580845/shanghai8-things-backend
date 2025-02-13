using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using SqlSugar;

namespace Hgzn.Mes.Iot.Mqtt
{
    public class IotMqttExplorer : IMqttExplorer
    {
        private readonly IManagedMqttClient _mqttClient;
        private readonly ManagedMqttClientOptions _mqttClientOptions;
        private readonly ISqlSugarClient _client;
        
        public IotMqttExplorer(
            ILogger<IotMqttExplorer> logger,
            IConfiguration configuration,
            ISqlSugarClient client,
            MqttMessageHandler mqttMessageParser)
        {
            _logger = logger;
            var mqtt = configuration.GetConnectionString("Mqtt")!.Split(':');
            _client = client; 
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
            _mqttClient.ConnectedAsync += async _ =>
            {
                _logger.LogInformation("mqtt connected");
                await PublishAsync(TopicBuilder.CreateBuilder()
                    .WithPrefix(TopicType.Sys)
                    .WithDirection(MqttDirection.Up)
                    .WithTag(MqttTag.State)
                    .Build(),
                    [(byte)MqttState.Connected]);
            };
            _mqttClient.DisconnectedAsync += async _ =>
                await Task.Run(() => { _logger.LogWarning("mqtt disconnected"); });
            _mqttClient.ApplicationMessageReceivedAsync += async (args) =>
            {
                try
                {
                    mqttMessageParser.Initialize(this);
                    await mqttMessageParser.HandleAsync(args.ApplicationMessage);
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case NotSupportedException:
                            break;

                        default:
                            _logger.LogError("deal mq data failure: " + ex);
                            break;
                    }
                }
            };
        }

        private readonly ILogger<IotMqttExplorer> _logger;

        public async Task StartAsync()
        {
            var topics = await GetSubscribeTopicsAsync();
            _logger.LogInformation($"restarting... subcribe to topics: {string.Join(";\r\n", topics.Select(t => t.Topic))}");
            await _mqttClient.SubscribeAsync(topics);
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        /// <summary>
        ///     以最新的设备类型表进行订阅
        /// </summary>
        /// <returns></returns>
        public async Task RestartAsync()
        {
            var topics = await GetSubscribeTopicsAsync();
            _logger.LogInformation($"restarting... subcribe to topics: {string.Join(";\r\n", topics.Select(t => t.Topic))}");
            await PublishAsync(TopicBuilder.CreateBuilder()
                .WithPrefix(TopicType.Sys)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.State)
                .Build(),
                [(byte)MqttState.Restart]);
            await _mqttClient.StopAsync();
            await _mqttClient.SubscribeAsync(topics);
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        public async Task<ICollection<MqttTopicFilter>> GetSubscribeTopicsAsync()
        {
            var types = await _client.Queryable<EquipType>().Select(dt => dt.TypeCode).ToArrayAsync();
            return types
                .Select(type => $"{TopicType.Iot:F}/+/{IotTopic.EquipTypeName}/{type}/{IotTopic.ConnUriName}/+/+".ToLower())
                .Select(topic => new MqttTopicFilterBuilder().WithTopic(topic)
                    .WithExactlyOnceQoS()
                    .Build()).ToArray();
        }

        public async Task StopAsync()
        {
            await PublishAsync(TopicBuilder.CreateBuilder()
                .WithPrefix(TopicType.Sys)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.State)
                .Build(),
                [(byte)MqttState.Stop]);
            await _mqttClient.StopAsync();
        }

        public async Task PublishAsync(string Topic, byte[] payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(Topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();
            await _mqttClient.EnqueueAsync(message);
        }

        public async Task UnSubscribeAsync(string topic)
        {
            await _mqttClient.SubscribeAsync(topic);
        }

        public async Task SubscribeAsync(string topic)
        {
            await _mqttClient.UnsubscribeAsync(topic);
        }

        public Task<bool> IsConnectedAsync()
        {
            return Task.FromResult(_mqttClient.IsConnected);
        }
    }
}