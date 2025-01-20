using Hgzn.Mes.Infrastructure.Mqtt;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.RfidReader;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;

namespace RfidReaderClient.mqtt;

public class MqttHelp
{
    private readonly ILogger<MqttHelp> _logger;
    private readonly IMqttExplorer _mqttExplorer;

    public MqttHelp(ILogger<MqttHelp> logger, IMqttExplorer mqttExplorer)
    {
        _logger = logger;
        this._mqttExplorer = mqttExplorer;
        mqttExplorer.MessageReceivedAsync += MqttExplorerOnMessageReceived;
    }

    public async Task StartAsync()
    {
        var rfid = new RfidReaderTopicBuilder()
            .WithPrefix(TopicTypeEnum.Equip)
            .WithDirection(MqttDirection.Down)
            .WithDeviceType(TopicEquipEnum.RfidReader)
            .WithEquipId("+")
            .WithTag(MqttTag.State)
            .Build();
        Console.WriteLine("注册   "+rfid);
        await _mqttExplorer.SubscribeAsync(rfid);
    }

    private Task MqttExplorerOnMessageReceived(MqttMessageEventArgs arg)
    {
        Console.WriteLine(arg.Topic);
        return Task.CompletedTask;
    }
}