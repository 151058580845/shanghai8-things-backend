using Hgzn.Mes.Infrastructure.Mqtt.RfidReader;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
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
    private ManagedMqttClientOptions _mqttClientOptions = null!;
    private readonly IManagedMqttClient _mqttClient;
    private readonly TimeSpan _reconnectDelay;
    private readonly HashSet<string> _subscribedTopics = new();
    private readonly MqttSettings _mqttSettings;
    public TopicBuilder WillTopicBuilder { get; set; } = new();

    public ApiMqttPub(ILogger<ApiMqttPub> logger, IManagedMqttClient mqttClient, MqttSettings mqttSettings)
    {
        _logger = logger;
        _mqttClient = mqttClient;
        _mqttSettings = mqttSettings;
        _reconnectDelay = TimeSpan.FromSeconds(_mqttSettings.ReconnectDelaySeconds);
        // 订阅客户端连接和断开连接的事件
        _mqttClient.ConnectedAsync += OnClientConnectedAsync;
        _mqttClient.DisconnectedAsync += OnClientDisconnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
    }

    public async Task StartAsync()
    {
        if (_mqttSettings is { Username: not null, Password: not null })
        {
            //连接mqtt服务器
            var options = CreateMqttClientOptions(_mqttSettings.Broker, _mqttSettings.Port, _mqttSettings.Username,
                _mqttSettings.Password);
            await _mqttClient.StartAsync(options);
        }

        while (!_mqttClient.IsConnected)
        {
            Task.Delay(100).Wait();
        }
        //订阅消息
        var rfid = new RfidReaderTopicBuilder()
            .WithPrefix(TopicTypeEnum.Equip)
            .WithDirection(MqttDirection.Down)
            .WithDeviceType(TopicEquipEnum.RfidReader)
            .WithEquipId("+")
            .WithTag(MqttTag.State)
            .Build();
        await SubscribeAsync(rfid);
        Console.WriteLine(rfid);
    }


    /// <summary>
    /// 启动Mqtt客户端并连接到服务器
    /// </summary>
    /// <param name="server"></param>
    /// <param name="port"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public async Task StartAsync(string server, int port, string username, string password)
    {
        var options = CreateMqttClientOptions(server, port, username, password);
        await _mqttClient.StartAsync(options);
    }

    /// <summary>
    /// 停止Mqtt服务器
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task StopAsync()
    {
        //创建停止消息
        var message = CreateMattMessage(MqttState.Stop);
        //发布停止消息
        await _mqttClient.EnqueueAsync(message);
        //停止Mqtt客户端
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
    public async Task RestartAsync(string server, int port, string? username, string? password)
    {
        // 使用新的连接选项重新创建客户端配置
        _mqttClientOptions = CreateMqttClientOptions(server, port, username, password);
        // 创建重启消息
        var message = CreateMattMessage(MqttState.Restart);
        // 发布重启消息
        await _mqttClient.EnqueueAsync(message);
        // 停止MQTT客户端并重新启动
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
        
        _subscribedTopics.Add(topic);
    }

    /// <summary>
    /// 发送一个主题数据
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    public async Task PublishAsync(string topic, string payload)
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
        _subscribedTopics.Add(topic);
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
        _subscribedTopics.Remove(topic);
    }


    /// <summary>
    /// 检查客户端的连接状态
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsConnectedAsync()
    {
        return Task.FromResult(_mqttClient.IsConnected);
    }

    public event Func<MqttClientConnectedEventArgs, Task>? MqttClientConnectedAsync;
    public event Func<MqttClientDisconnectedEventArgs, Task>? MqttClientDisconnectedAsync;
    public event Func<MqttMessageEventArgs, Task>? MessageReceivedAsync;

    /// <summary>
    /// 事件触发：客户端连接成功时调用
    /// </summary>
    /// <param name="args"></param>
    private Task OnClientConnectedAsync(MqttClientConnectedEventArgs args)
    {
        _logger.LogInformation("mqtt connected");
        MqttClientConnectedAsync?.Invoke(args);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 事件触发：客户端断开连接时调用
    /// </summary>
    /// <param name="args"></param>
    private Task OnClientDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        _logger.LogError("mqtt connected");
        MqttClientDisconnectedAsync?.Invoke(args);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 事件触发：接收到消息时调用
    /// </summary>
    /// <param name="args"></param>
    private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        if (args.ApplicationMessage.PayloadSegment.Count <= 0) return;
        var mqttMessageEventArgs = new MqttMessageEventArgs(args.ApplicationMessage.Topic,
            args.ApplicationMessage.PayloadSegment.ToArray());
        await MessageReceivedAsync?.Invoke(mqttMessageEventArgs)!;
        // return Task.CompletedTask;
    }


    /// <summary>
    /// 获取已经订阅的消息
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<string>> GetSubscribedTopicsAsync()
    {
        return Task.FromResult<IEnumerable<string>>(_subscribedTopics);
    }

    /// <summary>
    /// 创建一个Mqtt连接客户端
    /// </summary>
    /// <param name="server"></param>
    /// <param name="port"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    private ManagedMqttClientOptions CreateMqttClientOptions(string server, int port, string? username,
        string? password)
    {
        var mqttOptionsBuilder = new MqttClientOptionsBuilder()
            .WithClientId(_mqttSettings.ClientId)
            .WithKeepAlivePeriod(_reconnectDelay)
            .WithWillQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .WithTcpServer(server, port)
            .WithWillTopic(WillTopicBuilder
                .WithTag(MqttTag.State)
                .Build())
            .WithWillPayload([(byte)MqttState.Will])
            .WithCleanSession();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            mqttOptionsBuilder = mqttOptionsBuilder.WithCredentials(username, password);
        }

        var option = mqttOptionsBuilder
            .Build();
        _mqttClientOptions = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(_reconnectDelay)
            .WithClientOptions(option)
            .Build();
        return _mqttClientOptions;
    }

    /// <summary>
    /// 创建一个特定的消息
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private MqttApplicationMessage CreateMattMessage(MqttState state)
    {
        return new MqttApplicationMessageBuilder()
            .WithTopic(WillTopicBuilder
                .WithTag(MqttTag.State)
                .Build())
            .WithPayload([(byte)state])
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
    }
}