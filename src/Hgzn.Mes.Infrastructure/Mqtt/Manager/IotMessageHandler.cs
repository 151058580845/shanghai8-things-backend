using Hgzn.Mes.Domain.Entities.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enum;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_307.ZXWL_SL_1;
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
using Hangfire;
using Hgzn.Mes.Domain.Entities.Equip.Rfid;

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
                case MqttTag.Transmit:
                    await HandleTransmitAsync(topic, message.PayloadSegment.Array!);
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
                SendTime = DateTime.Now.ToLocalTime(),
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
                    _logger.LogDebug($"AG - ================= 我看看有没有给这个主题发送数据的 =================");
                    // 根据连接ID查询设备ID
                    EquipConnect con = await _client.Queryable<EquipConnect>().FirstAsync(x => x.Id == uri);
                    Guid equipId = con.EquipId;
                    OnlineReceiveDispatch dispatch =
                        new OnlineReceiveDispatch(equipId, _client, _connectionMultiplexer, _mqttExplorer);
                    await dispatch.Handle(msg);
                    break;
                case EquipConnType.RKServer:
                    // 处理RKServer设备数据
                    var rkData = JsonSerializer.Deserialize<HygrographData>(msg, Options.CustomJsonSerializerOptions);
                    if (rkData is null)
                    {
                        _logger.LogWarning("unexpected RKServer device msg");
                        return;
                    }

                    // 处理RKServer数据逻辑,这个数据本来是打算从后台内存中读取的,现在修改为根据MQTT推送的消息,在前端获取,所以现在随便传个数字过去就行,在前端改
                    // 更新:理论上可以,实际上不行,因为前端会刷新界面,前端刷新界面的时候可能MQTT还没有推送,所以会显示默认数据
                    await HandleRkServerDataAsync(uri, rkData);
                    break;
            }
        }

        private async Task HandleRkServerDataAsync(Guid uri, HygrographData rkData)
        {
            // 将推送过来的消息解析到静态内存中保存
            if (rkData.RoomId != null && rkData.RoomId != Guid.Empty && rkData.Temperature != null &&
                rkData.Humidness != null)
                RKData.RoomId_TemperatureAndHumidness[rkData.RoomId.Value] =
                    new Tuple<float, float>(rkData.Temperature.Value, rkData.Humidness.Value);
        }

        private async Task HandleTransmitAsync(IotTopic topic, byte[] msg)
        {
            _logger.LogDebug($"AG - webApi收到转发数据");
            var uri = Guid.Parse(topic.ConnUri!);
            var connType = topic.ConnType;
            if (connType == EquipConnType.IotServer)
            {
                // 根据连接ID查询设备ID
                EquipConnect con = await _client.Queryable<EquipConnect>().FirstAsync(x => x.Id == uri);
                Guid equipId = con.EquipId;
                OnlineReceiveDispatch dispatch =
                    new OnlineReceiveDispatch(equipId, _client, _connectionMultiplexer, _mqttExplorer);
                await dispatch.Handle(msg);
            }
        }

        private async Task HandleRfidMsgAsync(Guid uri, RfidMsg msg)
        {
            //var targetId = Guid.Parse(msg.Userdata ?? throw new ArgumentNullException("userdata not exist"));
            var label = await _client.Queryable<LocationLabel>()
                .Where(x => x.TagId == msg.Tid)
                .FirstAsync();
            if (label is null)
            {
                _logger.LogTrace("new label found");
                return;
            }

            if (label.EquipLedgerId is null)
            {
                _logger.LogInformation("label not binding to equip");
                return;
            }

            // 读写器所在房间
            var rfidReader = (await _client.Queryable<EquipConnect>()
                .Includes(ec => ec.EquipLedger, el => el!.Room)
                .Where(ec => ec.Id == uri)
                .Select(ec => new
                {
                    RfidId = ec.EquipLedger!.Id,
                    RfidName = ec.EquipLedger.EquipName,
                    ec.EquipLedger!.RoomId,
                    RoomName = ec.EquipLedger!.Room!.Name,
                    IsCustoms = ec.EquipLedger!.IsCustoms
                })
                .FirstAsync());
            if (rfidReader is null)
            {
                _logger.LogError("rfid device not bind to room");
                return;
            }

            // 查找标签所在设备
            var equip = await _client.Queryable<EquipLedger>()
                .Where(x => x!.Id == label!.EquipLedgerId)
                .Includes(eq => eq.Room)
                .FirstAsync();

            #region 判定设备之前所在位置，如果是当前房间，就不处理,留存时间为1天

            // 记录到redis
            IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            var key = string.Format(CacheKeyFormatter.EquipRoom, equip.Id);
            var value = redisDb.StringGet(key: key);
            if (value == rfidReader.RoomId.ToString())
            {
                return;
            }
            // 将时间戳存入Redis并设置1天过期时间
            await redisDb.StringSetAsync(key: key, value: rfidReader.RoomId.ToString(), expiry: TimeSpan.FromDays(1));
            #endregion
            
            // #region 判定设备之前所在位置，如果是当前房间，就不处理,留存时间为1天
            //
            // //当前记录
            // var nowEquipRfidTime = new EquipRfidTime()
            // {
            //     EquipId = equip.Id,
            //     RoomId = label.RoomId ?? Guid.Empty,
            //     CreateTime = DateTime.Now
            // };
            //
            // // 记录到redis
            // IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            // var key = string.Format(CacheKeyFormatter.EquipRoom, equip.Id);
            // var value = await redisDb.StringGetAsync(key: key); // 建议使用异步方法
            //
            // Queue<EquipRfidTime> equipList;
            // if (value.HasValue)
            // {
            //     try
            //     {
            //         // 尝试反序列化
            //         equipList = JsonSerializer.Deserialize<Queue<EquipRfidTime>>(value) ?? new Queue<EquipRfidTime>();
            //     }
            //     catch (JsonException ex)
            //     {
            //         _logger.LogError(ex, "Failed to deserialize EquipRfidTime from Redis");
            //         // 如果反序列化失败，初始化一个新的队列
            //         equipList = new Queue<EquipRfidTime>();
            //     }
            // }
            // else
            // {
            //     equipList = new Queue<EquipRfidTime>();
            // }
            //
            // // 如果队列不为空且和最新的房间ID一样，就取消处理
            // if (equipList.Count > 0 && equipList.Last().RoomId == nowEquipRfidTime.RoomId)
            // {
            //     return;
            // }
            // var recentRecords = equipList.Where(x => (DateTime.Now - x.CreateTime) < TimeSpan.FromMinutes(3)).ToList();
            // // 如果超过5条记录，删除最旧的一条
            // if (equipList.Count >= 5)
            // {
            //     equipList.Dequeue();
            // }
            //
            // equipList.Enqueue(nowEquipRfidTime);
            //
            // // 将数据存入Redis并设置1天过期时间
            // await redisDb.StringSetAsync(
            //     key: key,
            //     value: JsonSerializer.Serialize(equipList),
            //     expiry: TimeSpan.FromDays(1)
            // );
            // //如果房间就两个，就返回
            // if (recentRecords.Count <= 2)
            // {
            //     return;
            // }
            // #endregion


            
            
            #region 判断是否是关卡读写器,超过5分钟报警

            var jobKey = string.Format(CacheKeyFormatter.EquipAlarmJob, equip.Id);
            //获取到对应设备的JobId
            var job = redisDb.StringGet(key: key);
            if (job.HasValue && BackgroundJob.Delete(jobKey))
            {
                if (rfidReader.IsCustoms != null && rfidReader.IsCustoms.Value)
                {
                    // 处理关卡读写器逻辑
                    // 这里可以添加关卡读写器的处理逻辑
                    _logger.LogInformation($"被关卡读写器读取: {rfidReader.RfidName}");
                    // 添加定时任务
                    var jobId = BackgroundJob.Schedule(() => SentAlarmAsync(new EquipNotice
                    {
                        Id = Guid.NewGuid(),
                        EquipId = equip.Id,
                        SendTime = DateTime.Now,
                        Title = "关卡读写器读取",
                        Content = $"设备 {equip.EquipName} 在关卡读写器 {rfidReader.RfidName} 被读取",
                        Description = $"设备 {equip.EquipName} 在关卡读写器 {rfidReader.RfidName} 被读取",
                        NoticeType = EquipNoticeType.Alarm
                    }), TimeSpan.FromMinutes(2));
                    await redisDb.StringSetAsync(key: jobKey, value: jobId, expiry: TimeSpan.FromMinutes(10));
                }
            }

            #endregion

            var time = DateTime.Now;
            var record = new EquipLocationRecord
            {
                DateTime = time,
                Tid = msg.Tid,
                Userdata = msg.Userdata,
                EquipId = equip.Id,
                EquipName = equip.EquipName,
                CreatorId = User.DevUser.CreatorId,
                ConnectId = uri,
                RfidEquipId = rfidReader.RfidId,
                RfidEquipName = rfidReader.RfidName,
            };

            string? roomName = equip?.Room?.Name;
            //原先设备已绑定则解绑
            // 需求变更,现在没有搬出房间警告,只更新设备的房间状态
            if (false && equip?.RoomId is not null && equip.RoomId == rfidReader.RoomId)
            {
                if ((equip.LastMoveTime is null ||
                     (time - equip.LastMoveTime.Value).TotalSeconds > _pos_interval * 60))
                {
                    equip.RoomId = null;
                    roomName = null;
                    equip.IsMoving = true;
                    equip.LastMoveTime = time;
                }
            }
            else //未绑定设备绑定至新房间
            {
                if (rfidReader.RoomId != null)
                {
                    equip!.RoomId = rfidReader.RoomId;
                    equip.RoomIdSourceType = 2;
                    roomName = rfidReader.RoomName;
                    equip.LastMoveTime = time;
                }
                else
                {
                    _logger.LogInformation("roomID 为空");
                }
            }

            if (rfidReader.RoomId != null)
            {
                record.RoomId = equip?.RoomId;
                record.RoomName = roomName;
                await _mqttExplorer.PublishAsync(UserTopicBuilder
                    .CreateUserBuilder()
                    .WithPrefix(TopicType.App)
                    .WithDirection(MqttDirection.Up)
                    .WithTag(MqttTag.Notice)
                    .WithUri(label!.EquipLedgerId.ToString()!)
                    .Build(), Encoding.UTF8.GetBytes(roomName ?? ""));
                await _client.Updateable(equip).ExecuteCommandAsync();
                await _client.Insertable(record).ExecuteCommandAsync();
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

        private async Task HandleAlarmAsync(IotTopic topic, byte[] msg)
        {
            EquipNotice? notice = JsonSerializer.Deserialize<EquipNotice>(msg);
            await _client.Insertable(notice).ExecuteCommandAsync();
        }

        private async Task SentAlarmAsync(EquipNotice notice)
        {
            await _client.Insertable(notice).ExecuteCommandAsync();
            // 发送报警消息到MQTT
            var alarmTopic = UserTopicBuilder
                .CreateUserBuilder()
                .WithPrefix(TopicType.App)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.Alarm)
                .WithUri(notice.EquipId.ToString()!)
                .Build();
            var msg = JsonSerializer.Serialize(notice, Options.CustomJsonSerializerOptions);
            await _mqttExplorer.PublishAsync(alarmTopic, Encoding.UTF8.GetBytes(msg));
        }
    }
}