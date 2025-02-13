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

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager;

/// <summary>
/// Mqtt客户端实例
/// </summary>
public class ApiMqttExplorer : IMqttExplorer
{
    private readonly ILogger<ApiMqttExplorer> _logger;
    private readonly ManagedMqttClientOptions _mqttClientOptions = null!;
    private readonly IManagedMqttClient _mqttClient;
    private readonly ISqlSugarClient _client;

    public ApiMqttExplorer(
        ILogger<ApiMqttExplorer> logger,
        IConfiguration configuration,
        ISqlSugarClient client,
        IotMessageHandler messageHandler)
    {
        _logger = logger;
        _client = client;
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
            await Task.Run(() => { _logger.LogError("mqtt disconnected"); });
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
                        _logger.LogError("deal mq data failure: " + ex);
                        break;
                }
            }
        };
    }

    public async Task StartAsync()
    {
        var topics = await GetSubscribeTopicsAsync();
        _logger.LogInformation($"restarting... subcribe to topics: {string.Join(";\r\n", topics.Select(t => t.Topic))}");
        await _mqttClient.SubscribeAsync(topics);
        await _mqttClient.StartAsync(_mqttClientOptions);
    }

    /// <summary>
    /// 停止Mqtt服务器
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task StopAsync()
    {
        var message = new MqttApplicationMessageBuilder()
        .WithTopic(TopicBuilder.CreateBuilder()
                    .WithPrefix(TopicType.Sys)
                    .WithDirection(MqttDirection.Up)
                    .WithTag(MqttTag.State)
                    .Build())
        .WithPayload([(byte)MqttState.Stop])
        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
        .Build();
        await _mqttClient.EnqueueAsync(message);
        await _mqttClient.StopAsync();
    }

    /// <summary>
    /// 重启Mqtt客户端
    /// </summary>
    /// <param name="server"></param>
    /// <param name="port"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task RestartAsync()
    {
        var message = new MqttApplicationMessageBuilder()
        .WithTopic(TopicBuilder.CreateBuilder()
                    .WithPrefix(TopicType.Sys)
                    .WithDirection(MqttDirection.Up)
                    .WithTag(MqttTag.State)
                    .Build())
        .WithPayload([(byte)MqttState.Restart])
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .Build();
        await _mqttClient.EnqueueAsync(message);
        await _mqttClient.StopAsync();
        var topics = await GetSubscribeTopicsAsync();
        _logger.LogInformation($"restarting... subcribe to topics: {string.Join(";\r\n", topics.Select(t => t.Topic))}");
        await _mqttClient.SubscribeAsync(topics);
        await _mqttClient.StartAsync(_mqttClientOptions);
    }

    /// <summary>
    /// 发送一个主题数据
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    public async Task PublishAsync(string topic, byte[] payload)
    {
        if (!_mqttClient.IsConnected)
        {
            await _mqttClient.StartAsync(_mqttClientOptions);
        }
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .Build();
        await _mqttClient.EnqueueAsync(message);
    }

    /// <summary>
    /// 订阅一个主题
    /// </summary>
    /// <param name="topic"></param>
    public async Task SubscribeAsync(string topic)
    {
        await _mqttClient.SubscribeAsync(topic);
    }

    /// <summary>
    /// 取消订阅一个主题
    /// </summary>
    /// <param name="topic"></param>
    /// <returns></returns>
    public async Task UnSubscribeAsync(string topic)
    {
        await _mqttClient.UnsubscribeAsync(topic);
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

    /// <summary>
    /// 检查客户端的连接状态
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsConnectedAsync()
    {
        return Task.FromResult(_mqttClient.IsConnected);
    }
}