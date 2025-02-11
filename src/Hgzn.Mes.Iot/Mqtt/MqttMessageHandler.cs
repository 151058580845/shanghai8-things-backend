using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Message;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Iot.EquipManager;
using Microsoft.Extensions.Logging;
using MQTTnet;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Hgzn.Mes.Iot.Mqtt
{
    public class MqttMessageHandler
    {
        public MqttMessageHandler(
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<MqttMessageHandler> logger,
            ConnManager manager)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
            _manager = manager;
        }

        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ConnManager _manager;
        private readonly ILogger<MqttMessageHandler> _logger;
        private IMqttExplorer _mqttExplorer = null!;
        private static ConcurrentDictionary<string, List<(int heart, int breath)>> _rawDataPackage = new();
        private const int countIndex = 60;

        public void Initialize(IMqttExplorer mqttExplorer)
        {
            _manager.Initialize(mqttExplorer);
            _mqttExplorer = mqttExplorer;
        }

        public async Task HandleAsync(MqttApplicationMessage message)
        {
            var topic = IotTopic.FromIotString(message.Topic);
            if (topic.Direction == MqttDirection.Down) return;
            switch (topic.Tag)
            {
                case MqttTag.State:
                    var state = new DeviceStateMsg(message.ConvertPayloadToString());
                    await HandleStateAsync(topic, state);
                    break;

                case MqttTag.Data:
                    await HandleDataAsync(topic, message.PayloadSegment.Array!);
                    break;
                    
                case MqttTag.Cmd:
                    var conn = JsonSerializer.Deserialize<ConnInfo>(message.ConvertPayloadToString());
                    await HandleCmdAsync(topic, conn!);
                    break;

                case MqttTag.Health:
                    //var health = new DeviceHealthMsg(message.ConvertPayloadToString());
                    //await HandleHealthAsync(topic, health, dbContext);
                    break;

                case MqttTag.Calibration:
                    await HandleCalibrationAsync(topic);
                    break;

                default:
                    return;
            }
        }

        private async Task HandleStateAsync(IotTopic topic, DeviceStateMsg message)
        {
            switch (topic.DeviceType)
            {
                case "rfidReader":
                    //await EquipControlHelp.AddDeviceManagerAsync(Guid.Parse(topic.DeviceUri),new RfidReaderManages(message.ToString(), _redisService));
                    break;
            }
        }

        private Task HandleDataAsync(IotTopic topic, byte[] msg)
        {
            throw new NotImplementedException();
        }

        private async Task HandleCmdAsync(IotTopic topic, ConnInfo info)
        {
            var uri = Guid.Parse(topic.DeviceUri!);

            switch (info.Type)
            {
                case CmdType.Conn:

                    var equip = _manager.GetEquip(uri) ?? _manager.AddEquip(uri, info.ConnType);
                    await equip.ConnectAsync(info);
                    await equip.StartAsync();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(info.Type));
            }

            
        }

        private void HandleHealthAsync(IotTopic topic,
            DeviceHealthMsg message)
        {
            throw new NotImplementedException();
        }

        private async Task HandleCalibrationAsync(IotTopic topic)
        {
            var newTopic = topic;
            newTopic.Direction = MqttDirection.Down;
            await _mqttExplorer.PublishAsync(newTopic.ToString(), new DeviceCalibrationMsg().AsFrame());
            _logger.LogInformation($"device: {topic.DeviceType} time calibrate succeed");
        }
    }
}