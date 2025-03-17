using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enum;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Mqtt.Message;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MySqlX.XDevAPI;
using NetCoreServer;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager
{
    public class IotMessageHandler
    {
        public IotMessageHandler(
            ILogger<IotMessageHandler> logger,
            SqlSugarContext context,
            IConnectionMultiplexer connectionMultiplexer,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _client = context.DbContext;
            _connectionMultiplexer = connectionMultiplexer;
            _pos_interval = configuration.GetValue<int?>("PosInterval") ?? 3;
        }

        private readonly ILogger<IotMessageHandler> _logger;
        private readonly ISqlSugarClient _client;
        private IMqttExplorer _mqttExplorer = null!;
        private static ConcurrentDictionary<string, List<(int heart, int breath)>> _rawDataPackage = new();
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly int _pos_interval;
        private const int countIndex = 60;

        public void Initialize(IMqttExplorer mqttExplorer)
        {
            _mqttExplorer = mqttExplorer;
        }

        public async Task HandleAsync(MqttApplicationMessage message)
        {
            var topic = IotTopic.FromIotString(message.Topic);
            if (topic.Direction == MqttDirection.Down) return;
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
                case MqttTag.Alarm:
                    await HandleAlarmAsync(topic, message.PayloadSegment.Array!);
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
            var connType = topic.ConnType;
            Guid? dataId = null;
            switch (connType)
            {
                case EquipConnType.RfidReader:
                    var rfid = JsonSerializer.Deserialize<RfidMsg>(msg, Options.CustomJsonSerializerOptions);
                    if (rfid is null)
                    {
                        _logger.LogWarning("unexpected device msg");
                        return;
                    }
                    await HandleRfidMsgAsync(uri, rfid);
                    break;
                case EquipConnType.IotServer:
                    // 根据连接ID查询设备ID
                    EquipConnect con = await _client.Queryable<EquipConnect>().FirstAsync(x => x.Id == uri);
                    Guid equipId = con.EquipId;
                    TestDataOnlineReceive testDataReceive = new TestDataOnlineReceive(equipId, _client, _connectionMultiplexer, _mqttExplorer);
                    dataId = await testDataReceive.Handle(msg, false);
                    break;
            }

        }

        private async Task HandleRfidMsgAsync(Guid uri, RfidMsg msg)
        {
            //var targetId = Guid.Parse(msg.Userdata ?? throw new ArgumentNullException("userdata not exist"));
            var label = await _client.Queryable<LocationLabel>()
                .Where(x => x.TagId == msg.Tid)
                .FirstAsync();
            if(label is null)
            {
                _logger.LogTrace("new label found");
                return;
            }
            if(label.EquipLedgerId is null)
            {
                _logger.LogInformation("label not binding to equip");
                return;
            }
            // 读写器所在房间
            var rfidRoom = (await _client.Queryable<EquipConnect>()
                .Includes(ec => ec.EquipLedger, el => el.Room)
                .Where(ec => ec.Id == uri)
                .Select(ec => new
                {
                    ec.EquipLedger.RoomId,
                    ec.EquipLedger.Room.Name,
                })
                .FirstAsync());
            if (rfidRoom is null)
            {
                _logger.LogError("rfid device not bind to room");
                return;
            }
            // 查找标签所在设备
            var equip = await _client.Queryable<EquipLedger>()
                .Where(x => x!.Id == label!.EquipLedgerId)
                .Includes(eq => eq.Room)
                .FirstAsync();

            string? roomName = equip?.Room?.Name;
            //原先设备已绑定则解绑
            if (equip.RoomId is not null && equip.RoomId == rfidRoom.RoomId)
            {
                if ((equip.LastMoveTime is null ||
                (DateTime.UtcNow - equip.LastMoveTime.Value).TotalSeconds > _pos_interval * 60))
                {
                    equip.RoomId = null;
                    roomName = null;
                    equip.IsMoving = true;
                    equip.LastMoveTime = DateTime.UtcNow;
                }
            }
            else//未绑定设备绑定至新房间
            {
                equip.RoomId = rfidRoom.RoomId;
                roomName = rfidRoom.Name;
                equip.LastMoveTime = DateTime.UtcNow;
            }
            await _mqttExplorer.PublishAsync(UserTopicBuilder
            .CreateUserBuilder()
            .WithPrefix(TopicType.App)
            .WithDirection(MqttDirection.Up)
            .WithTag(MqttTag.Notice)
            .WithUri(label!.EquipLedgerId.ToString()!)
            .Build(), Encoding.UTF8.GetBytes(roomName ?? ""));
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

        private async Task HandleAlarmAsync(IotTopic topic, byte[] msg)
        {
            EquipNotice? notice = JsonSerializer.Deserialize<EquipNotice>(msg);
            await _client.Insertable(notice).ExecuteCommandAsync();
        }
    }
}