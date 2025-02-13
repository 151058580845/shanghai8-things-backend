using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Hgzn.Mes.Infrastructure.Mqtt.Message;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceive;
using Microsoft.Extensions.Logging;
using MQTTnet;
using SqlSugar;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text.Json;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager
{
    public class IotMessageHandler
    {
        public IotMessageHandler(
            ILogger<IotMessageHandler> logger,
            ISqlSugarClient client)
        {
            _logger = logger;
            _client = client;
        }

        private readonly ILogger<IotMessageHandler> _logger;
        private readonly ISqlSugarClient _client;
        private IMqttExplorer _mqttExplorer = null!;
        private static ConcurrentDictionary<string, List<(int heart, int breath)>> _rawDataPackage = new();
        private const int countIndex = 60;

        public void Initialize(IMqttExplorer mqttExplorer)
        {
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
            var connId = Guid.Parse(topic.ConnUri!);
            var equipId = (await _client.Queryable<EquipConnect>().FirstAsync(t => t.Id == connId)).EquipId;
            _client.Insertable(new EquipNotice()
            {
                Id = Guid.NewGuid(),
                EquipId = equipId,
                SendTime = DateTime.Now,
                Title = "设备连接操作",
                Content = "",
                Description = "",
                NoticeType = message.State
            });
            switch (topic.EquipType)
            {
                case "rfidReader":
                    //await EquipControlHelp.AddDeviceManagerAsync(Guid.Parse(topic.DeviceUri),new RfidReaderManages(message.ToString(), _redisService));
                    break;
            }
        }

        private async Task HandleDataAsync(IotTopic topic, byte[] msg)
        {
            var uri = Guid.Parse(topic.ConnUri!);
            switch (topic.EquipType)
            {
                case "rfid-reader":
                    var rfid = JsonSerializer.Deserialize<RfidMsg>(msg, Options.CustomJsonSerializerOptions);
                    if (rfid is null)
                    {
                        _logger.LogWarning("unexpected device msg");
                        return;
                    }
                    await HandleRfidMsgAsync(uri, rfid);
                    break;
                case "tcp-server":
                    TestDataReceive testDataReceive = new TestDataReceive(_client);
                    await testDataReceive.Handle(msg);
                    break;

            }
        }
        private async Task HandleRfidMsgAsync(Guid uri, RfidMsg msg)
        {
            var targetId = Guid.Parse(msg.Userdata ?? throw new ArgumentNullException("userdata not exist"));
            // 读写器所在房间
            var rfidRoom = await _client.Queryable<EquipLedger>()
                .Select(el => el.RoomId)
                .FirstAsync(id => id == uri);
            if (rfidRoom is null)
            {
                _logger.LogError("rfid device not bind to room");
                return;
            }
            // 查找标签所在设备
            var equip = await _client.Queryable<EquipLedger>()
                .Includes(eq => eq.Room)
                .FirstAsync(x => x!.Id == targetId);

            var tids = equip.PosTags is null ? null : equip.PosTags.Split(';');

            //原先设备已绑定则解绑
            if (equip.RoomId is not null)
            {
                equip.RoomId = null;
                equip.IsMoving = true;
                equip.LastMoveTime = DateTime.UtcNow;
            }
            else //未绑定设备绑定至新房间
            {
                equip.RoomId = rfidRoom;
                equip.LastMoveTime = DateTime.UtcNow;
            }

            await _client.Updateable(equip).ExecuteCommandAsync();
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