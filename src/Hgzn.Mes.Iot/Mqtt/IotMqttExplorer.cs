using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
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
        private ICollection<MqttTopicFilter> _mqttTopics = null!;
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
            var types = _client.Queryable<EquipType>().Select(dt => dt.TypeCode).ToArray();
            RefreshTopicsAsync();
            _logger.LogInformation($"now subcribe to device type: {string.Join(',', types)}");
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
                    .WithWillPayload(new byte[] { (byte)MqttState.Will })
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
                    new byte[] { (byte)MqttState.Connected });
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
            await _mqttClient.SubscribeAsync(_mqttTopics);
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        public async Task ResetAsync(params string[] topics)
        {
            var mqttTopics = topics
                .Select(topic => new MqttTopicFilterBuilder().WithTopic(topic)
                    .WithExactlyOnceQoS()
                    .Build()).ToArray();
            await PublishAsync(TopicBuilder.CreateBuilder()
                .WithPrefix(TopicType.Sys)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.State)
                .Build(),
                new byte[] { (byte)MqttState.Restart });
            await _mqttClient.StopAsync();
            await _mqttClient.SubscribeAsync(mqttTopics);
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        /// <summary>
        ///     以最新的设备类型表进行订阅
        /// </summary>
        /// <returns></returns>
        public async Task RestartAsync()
        {
            var types = _client.Queryable<EquipType>().Select(dt => dt.TypeCode).ToArray();
            await RefreshTopicsAsync();
            _logger.LogInformation($"restarting... subcribe to device type: {string.Join(',', types)}");
            await PublishAsync(TopicBuilder.CreateBuilder()
                .WithPrefix(TopicType.Sys)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.State)
                .Build(),
                new byte[] { (byte)MqttState.Restart });
            await _mqttClient.StopAsync();
            await _mqttClient.SubscribeAsync(_mqttTopics);
            await _mqttClient.StartAsync(_mqttClientOptions);
        }

        public Task RefreshTopicsAsync(params string[] topics)
        {
            var types = _client.Queryable<EquipType>().Select(dt => dt.TypeCode).ToArray();
            _mqttTopics = types
                .Select(type => $"{TopicType.Iot:F}/+/{IotTopic.EquipTypeName}/{type}/{IotTopic.ConnUriName}/+/+".ToLower())
                .Select(topic => new MqttTopicFilterBuilder().WithTopic(topic)
                    .WithExactlyOnceQoS()
                    .Build()).ToArray();
            return Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            await PublishAsync(TopicBuilder.CreateBuilder()
                .WithPrefix(TopicType.Sys)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.State)
                .Build(),
                new byte[] { (byte)MqttState.Stop });
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

        public Task UnSubscribeAsync(string topic)
        {
            throw new NotImplementedException();
        }

        public Task SubscribeAsync(string topic)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsConnectedAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateStateAsync(ConnStateType stateType, string uri)
        {
            await PublishAsync(UserTopicBuilder
            .CreateUserBuilder()
            .WithPrefix(TopicType.App)
            .WithDirection(MqttDirection.Up)
            .WithTag(MqttTag.State)
            .WithUri(uri!)
            .Build(), BitConverter.GetBytes((int)stateType));
        }
    }
}