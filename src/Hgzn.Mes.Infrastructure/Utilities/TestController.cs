using System.Text;
using System.Text.Json;
using Hangfire;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.Equip.Rfid;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using SqlSugar;
using StackExchange.Redis;
using IDatabase = Microsoft.EntityFrameworkCore.Storage.IDatabase;

namespace Hgzn.Mes.Infrastructure.Utilities;

public class TestController
{
    private readonly ISqlSugarClient _client;
    private readonly IMqttExplorer _mqttExplorer;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public TestController(ISqlSugarClient client, IMqttExplorer mqttExplorer , IConnectionMultiplexer connectionMultiplexer)
    {
        _client = client;
        _mqttExplorer = mqttExplorer;
        _connectionMultiplexer = connectionMultiplexer;
    }
    
    public async Task equipStart()
    {
        var equipId = Guid.Parse("0e8fe514-1171-4eac-ba91-7482d15996eb");
        var equip = await _client.Queryable<EquipLedger>().FirstAsync(t => t.Id == equipId);

        var rfidReaderId = Guid.Parse("862f4d02-e411-4213-8c7b-ef4298aa8392");
        var rfidReader = await _client.Queryable<EquipLedger>().FirstAsync(t => t.Id == rfidReaderId);
        Console.WriteLine("222222222222222222222222222222" + equipId);

        var jobId = $"{equipId}";  // Ensure unique jobId

        //当前记录
        var nowEquipRfidTime = new EquipRfidTime()
        {
            EquipId = equip.Id,
            RoomId = Guid.Empty, // 这里假设没有房间ID
            CreateTime = DateTime.Now
        };

        // 记录到redis
        StackExchange.Redis.IDatabase redisDb = _connectionMultiplexer.GetDatabase();
        var key = string.Format(CacheKeyFormatter.EquipRoom, equip.Id);
        var value = await redisDb.StringGetAsync(key: key); // 建议使用异步方法

        Queue<EquipRfidTime> equipList;
        if (value.HasValue)
        {
            equipList = JsonSerializer.Deserialize<Queue<EquipRfidTime>>(value) ?? new Queue<EquipRfidTime>();
        }
        else
        {
            equipList = new Queue<EquipRfidTime>();
        }

        // 如果队列不为空且和最新的房间ID一样，就取消处理
        // if (equipList.Count > 0 && equipList.Last().RoomId == nowEquipRfidTime.RoomId)
        // {
        //     return;
        // }
        var recentRecords = equipList.Where(x => (DateTime.Now - x.CreateTime) < TimeSpan.FromMinutes(3)).ToList();
        // 如果超过5条记录，删除最旧的一条
        if (equipList.Count >= 5)
        {
            equipList.Dequeue();
        }

        equipList.Enqueue(nowEquipRfidTime);

        // 将数据存入Redis并设置1天过期时间
        await redisDb.StringSetAsync(
            key: key,
            value: JsonSerializer.Serialize(equipList),
            expiry: TimeSpan.FromDays(1)
        );
        //如果房间就两个，就返回
        if (recentRecords.Count <= 2)
        {
            return;
        }
    }

    public void SentAlarm22Async(EquipNotice notice)
    {
        // 一次性任务，无需删除定期任务
        Console.WriteLine("11111111111111111111111111111111111111" + notice.EquipId);
        RecurringJob.RemoveIfExists(notice.EquipId.ToString());
        // 发送报警消息到MQTT
        var alarmTopic = UserTopicBuilder
            .CreateUserBuilder()
            .WithPrefix(TopicType.App)
            .WithDirection(MqttDirection.Up)
            .WithTag(MqttTag.Alarm)
            .WithUri(notice.EquipId.ToString()!)
            .Build();
        var msg = JsonSerializer.Serialize(notice, Options.CustomJsonSerializerOptions);
        _mqttExplorer.PublishAsync(alarmTopic, Encoding.UTF8.GetBytes(msg));
    }

}