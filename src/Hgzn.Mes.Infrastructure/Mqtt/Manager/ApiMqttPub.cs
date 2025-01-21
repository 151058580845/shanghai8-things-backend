using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager;

/// <summary>
/// Mqtt客户端实例
/// </summary>
public class ApiMqttPub : IMqttExplorer
{
    private readonly ILogger<ApiMqttPub> _logger;
    private readonly ManagedMqttClientOptions _mqttClientOptions = null!;
    private readonly IManagedMqttClient _mqttClient;

    public ApiMqttPub(ILogger<ApiMqttPub> logger, IConfiguration configuration)
    {
        _logger = logger;
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
                .WithWillPayload(new byte[] { (byte)MqttState.Will })
                .WithCleanSession()
                .Build())
            .Build();
        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateManagedMqttClient();
        _mqttClient.DisconnectedAsync += async _ =>
            await Task.Run(() => { _logger.LogError("mqtt disconnected"); });
    }

    public async Task StartAsync()
    {
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
        .WithPayload(new byte[] { (byte)MqttState.Stop })
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
        .WithPayload(new byte[] { (byte)MqttState.Restart })
        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
        .Build();
        await _mqttClient.EnqueueAsync(message);
        await _mqttClient.StopAsync();
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


    /// <summary>
    /// 检查客户端的连接状态
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsConnectedAsync()
    {
        return Task.FromResult(_mqttClient.IsConnected);
    }
}