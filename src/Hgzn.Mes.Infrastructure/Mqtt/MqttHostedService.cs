using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;

namespace Hgzn.Mes.Infrastructure.Mqtt;

/// <summary>
/// Mqtt服务
/// </summary>
public class MqttHostedService:IHostedService
{
    private readonly ILogger<MqttHostedService> _logger;
    private readonly IMqttClient _mqttClient;
    private readonly MqttSettings _mqttSettings;

    public MqttHostedService(ILogger<MqttHostedService> logger, IMqttClient mqttClient, MqttSettings mqttSettings)
    {
        _logger = logger;
        _mqttClient = mqttClient;
        _mqttSettings = mqttSettings;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var mqttOptionsBuilder = new MqttClientOptionsBuilder()
            .WithClientId(_mqttSettings.ClientId)
            .WithTcpServer(_mqttSettings.Broker, _mqttSettings.Port)
            .WithCleanSession();
        if (!string.IsNullOrEmpty(_mqttSettings.Username))
        {
            mqttOptionsBuilder = mqttOptionsBuilder.WithCredentials(_mqttSettings.Username, _mqttSettings.Password);
        }

        if (_mqttSettings.UseTls)
        {
            mqttOptionsBuilder = mqttOptionsBuilder.WithTlsOptions(new MqttClientTlsOptions()
            {
                UseTls = true,
            });
        }
        
        var mqttOptions = mqttOptionsBuilder.Build();
        await _mqttClient.ConnectAsync(mqttOptions, cancellationToken);
        _mqttClient.ConnectingAsync += MqttClientOnConnectingAsync;
        _mqttClient.ConnectedAsync += MqttClientOnConnectedAsync;
        _mqttClient.DisconnectedAsync += MqttClientOnDisconnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += MqttClientOnApplicationMessageReceivedAsync;
        _mqttClient.InspectPacketAsync += MqttClientOnInspectPacketAsync;
    }

    private async Task MqttClientOnInspectPacketAsync(InspectMqttPacketEventArgs arg)
    {
        throw new NotImplementedException();
    }

    private async Task MqttClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        throw new NotImplementedException();
    }

    private async Task MqttClientOnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        _logger.LogInformation("Mqtt断开连接");
    }

    private async Task MqttClientOnConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        _logger.LogInformation("Mqtt连接成功");

    }

    private async Task MqttClientOnConnectingAsync(MqttClientConnectingEventArgs arg)
    {
        _logger.LogInformation("Mqtt连接中");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
        }
    }
}