using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Message;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceive;
using Hgzn.Mes.Iot.EquipManager;
using Microsoft.Extensions.Logging;
using MQTTnet;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Hgzn.Mes.Iot.Mqtt
{
    public class MqttMessageHandler
    {
        public MqttMessageHandler(
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<MqttMessageHandler> logger,
            ISqlSugarClient client,
            ConnManager manager)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
            _client = client;
            _manager = manager;
        }

        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ConnManager _manager;
        private readonly ILogger<MqttMessageHandler> _logger;
        ISqlSugarClient _client;
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
            if (topic.Direction == MqttDirection.Up) return;
            switch (topic.Tag)
            {
                case MqttTag.State:
                    var state = new DeviceStateMsg(message.PayloadSegment);
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
            var type = Enum.Parse<ConnType>(topic.EquipType!);
            switch (type)
            {
                case ConnType.ModbusRtu:
                    break;
                case ConnType.ModbusAscii:
                    break;
                case ConnType.ModbusTcp:
                    break;
                case ConnType.ModbusUdp:
                    break;
                case ConnType.Http:
                    break;
                case ConnType.Socket:
                    break;
                case ConnType.SerialPort:
                    break;
                case ConnType.TcpServer:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            switch (topic.EquipType)
            {
                case "rfidReader":
                    //await EquipControlHelp.AddDeviceManagerAsync(Guid.Parse(topic.DeviceUri),new RfidReaderManages(message.ToString(), _redisService));
                    break;
            }
        }

        private async Task HandleDataAsync(IotTopic topic, byte[] msg)
        {
        }

        private async Task HandleCmdAsync(IotTopic topic, ConnInfo info)
        {
            var uri = Guid.Parse(topic.ConnUri!);

            switch (info.Type)
            {
                case CmdType.Conn:
                    var equip = _manager.GetEquip(uri) ?? _manager.AddEquip(uri, topic.ConnType!, info.ConnString!);
                    await SwitchEquipAsync(equip);
                    break;
                case CmdType.Collection:
                    var equipCon = _manager.GetEquip(uri) ?? _manager.AddEquip(uri, topic.ConnType!, info.ConnString!);
                    await CollectionDataAsync(equipCon);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(info.Type));
            }

            async Task SwitchEquipAsync(IEquipConnector equip)
            {
                var task = info.StateType switch
                {
                    ConnStateType.On => equip.ConnectAsync(info),
                    ConnStateType.Run => equip.StartAsync(),
                    ConnStateType.Stop => equip.StopAsync(),
                    ConnStateType.Off => equip.CloseConnectionAsync(),
                    _ => throw new NotImplementedException()
                };
                await task;
            }

            async Task CollectionDataAsync(IEquipConnector equip)
            {
                
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
            _logger.LogInformation($"device: {topic.EquipType} time calibrate succeed");
        }
    }
}