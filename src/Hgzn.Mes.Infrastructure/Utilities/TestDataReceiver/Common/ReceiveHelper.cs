using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common
{
    public static class ReceiveHelper
    {
        public static readonly ConcurrentQueue<byte[]> ReceiveTestSystem = new();

        private const int BodyStartIndex = 13;
        public static bool GetMessage(byte[] buffer, out uint bufferLength, out DateTime time, out byte[] message)
        {
            //标准头
            var header = buffer[0];
            if (header != 0x5A)
            {
                LoggerAdapter.LogWarning("报头符错误。");
                time = DateTime.MinValue;
                message = null!;
                bufferLength = 0;
                return false;
            }
            LoggerAdapter.LogInformation($"AG - 远程/本地通用 - 报头符:{header}");

            //报文长度
            bufferLength = BitConverter.ToUInt32(buffer, 1);
            LoggerAdapter.LogInformation($"AG - 远程/本地通用 - 报文长度:{bufferLength}");

            //解析报文流水号（1字节）
            byte number = buffer[5];
            LoggerAdapter.LogInformation($"AG - 远程/本地通用 - 报文流水号:{number}");

            // 解析时间
            byte[] bYear = new byte[2];
            Buffer.BlockCopy(buffer, 6, bYear, 0, 2);
            ushort year = BitConverter.ToUInt16(bYear, 0);

            byte month = buffer[8];
            byte day = buffer[9];
            byte hour = buffer[10];
            byte minute = buffer[11];
            byte second = buffer[12];
            LoggerAdapter.LogInformation($"AG - 远程/本地通用 - 时间:{year}-{month}-{day} {hour}:{minute}:{second}");

            //报文数据
            var length = bufferLength - 13;
            message = new byte[length];
            Buffer.BlockCopy(buffer, BodyStartIndex, message, 0, message.Length);
            time = new DateTime(year, month, day, hour, minute, second);
            LoggerAdapter.LogInformation($"AG - 远程/本地通用 - 仅正文长度:{length}");
            LoggerAdapter.LogInformation($"AG - 远程/本地通用 - 仅正文数据: {BitConverter.ToString(message, 0, (int)length).Replace("-", " ")}");

            return true;
        }

        public static async Task<EquipNotice> ExceptionRecordToLocalDB(ISqlSugarClient sqlSugarClient, Guid equipId, List<string> exception)
        {
            EquipNotice equipNotice = null!;
            if (exception.Any())
            {
                equipNotice = new EquipNotice()
                {
                    EquipId = equipId,
                    SendTime = DateTime.Now.ToLocalTime(),
                    NoticeType = EquipNoticeType.Alarm,
                    Title = "Receive Alarm",
                    Content = JsonConvert.SerializeObject(exception),
                    Description = "",
                };
                // 将异常记录到数据库
                equipNotice.Id = Guid.NewGuid();
                EquipNotice sequipNotice = await sqlSugarClient.Insertable(equipNotice).ExecuteReturnEntityAsync();
            }
            return equipNotice;
        }

        /// <summary>
        /// 将心跳记录到redis
        /// </summary>
        /// <param name="connectionMultiplexer"></param>
        /// <param name="simuTestSysId"></param>
        /// <param name="devTypeId"></param>
        /// <param name="compId"></param>
        /// <param name="equipId"></param>
        /// <param name="exception"></param>
        /// <param name="sendTime"></param>
        /// <param name="runTime"></param>
        /// <returns></returns>
        public static async Task LiveRecordToRedis(IConnectionMultiplexer connectionMultiplexer, ISqlSugarClient sqlSugarClient, byte simuTestSysId, byte devTypeId, string compNumber, DateTime sendTime)
        {
            // 记录到redis
            IDatabase redisDb = connectionMultiplexer.GetDatabase();
            Guid equipIdToUse = Guid.Empty;
            try
            {
                // 使用 ToListAsync().FirstOrDefault() 而不是 FirstAsync，避免未找到记录时抛异常
                var ledgers = await sqlSugarClient.Queryable<EquipLedger>()
                    .Where(x => x.AssetNumber == compNumber && !x.SoftDeleted)
                    .ToListAsync();
                var ledger = ledgers.FirstOrDefault();
                if (ledger != null)
                    equipIdToUse = ledger.Id;
            }
            catch (Exception ex)
            {
                LoggerAdapter.LogWarning($"AG - 异常记录 - 通过compId映射equipId失败，回退使用传入的equipId。原因: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
            // 记录异常信息
            var key = string.Format(CacheKeyFormatter.EquipLive, simuTestSysId, devTypeId, equipIdToUse);
            // 记录心跳时间（使用Unix时间戳格式）
            long timestamp = new DateTimeOffset(sendTime).ToUnixTimeSeconds();
            // 将时间戳存入Redis并设置30秒过期时间
            await redisDb.StringSetAsync(key: key, value: timestamp.ToString(), expiry: TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// 将异常记录到redis
        /// </summary>
        /// <param name="connectionMultiplexer"></param>
        /// <param name="sqlSugarClient"></param>
        /// <param name="simuTestSysId"></param>
        /// <param name="devTypeId"></param>
        /// <param name="compId"></param>
        /// <param name="equipId"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task ExceptionRecordToRedis(IConnectionMultiplexer connectionMultiplexer, ISqlSugarClient sqlSugarClient, byte simuTestSysId, byte devTypeId, byte[] compId, Guid equipId, List<string> exception, DateTime sendTime, uint runTime)
        {
            // 记录到redis
            IDatabase redisDb = connectionMultiplexer.GetDatabase();

            // 优先根据compId在台账中查找设备，使用其Id作为Redis键的equipId
            Guid equipIdToUse = equipId;
            try
            {
                string compNumber = Encoding.ASCII.GetString(compId).Trim('\0').Trim('"');
                LoggerAdapter.LogInformation($"AG - 异常记录 - 尝试通过compId映射equipId，compNumber: [{compNumber}], 原始equipId: {equipId}");

                // 使用 ToListAsync().FirstOrDefault() 而不是 FirstAsync，避免未找到记录时抛异常
                var ledgers = await sqlSugarClient.Queryable<EquipLedger>()
                    .Where(x => x.AssetNumber == compNumber && !x.SoftDeleted)
                    .ToListAsync();
                var ledger = ledgers.FirstOrDefault();

                if (ledger != null)
                {
                    equipIdToUse = ledger.Id;
                    LoggerAdapter.LogInformation($"AG - 异常记录 - 成功通过compId映射equipId，compNumber: [{compNumber}], 原始equipId: {equipId}, 映射后equipIdToUse: {equipIdToUse}, 台账设备名称: {ledger.EquipName}, 台账设备ID: {ledger.Id}");

                }
                else
                {
                    LoggerAdapter.LogWarning($"AG - 异常记录 - 未找到匹配的台账记录，compNumber: [{compNumber}], 查询结果数量: {ledgers?.Count ?? 0}, 使用原始equipId: {equipId}");
                }
            }
            catch (Exception ex)
            {
                LoggerAdapter.LogWarning($"AG - 异常记录 - 通过compId映射equipId失败，回退使用传入的equipId。原因: {ex.Message}, StackTrace: {ex.StackTrace}");
            }

            // 记录异常信息
            var key = string.Format(CacheKeyFormatter.EquipHealthStatus, simuTestSysId, devTypeId, equipIdToUse);
            LoggerAdapter.LogInformation($"AG - 异常记录 - 准备写入Redis，最终使用的equipIdToUse: {equipIdToUse}, Redis Key: {key}");
            await redisDb.KeyDeleteAsync(key);
            foreach (string item in exception)
            {
                await redisDb.SetAddAsync(key, item);
            }
            // 记录运行时长
            var runTimeKey = string.Format(CacheKeyFormatter.EquipRunTime, simuTestSysId, devTypeId, equipIdToUse);
            LoggerAdapter.LogInformation($"AG - 异常记录 - 准备写入运行时长，最终使用的equipIdToUse: {equipIdToUse}, Redis Key: {runTimeKey}");
            await ReportRunningTimeAsync(redisDb, runTimeKey, sendTime, runTime);
            await CleanupOldDataAsync(redisDb, runTimeKey);
        }

        public static async Task ReportRunningTimeAsync(IDatabase redisDb, string key, DateTime sendTime, uint accumulatedSeconds)
        {
            var now = sendTime;
            var today = now.ToString("yyyy-MM-dd");

            // 1. 更新 Redis HASH（记录当天最新秒数）
            await redisDb.HashSetAsync(
                key: $"{key}:days",
                hashField: today,
                value: accumulatedSeconds
            );

            // 2. 更新 Redis ZSET（记录活跃日期，用于后续清理）
            int dateNumber = int.Parse(now.Date.ToString("yyyyMMdd"));
            await redisDb.SortedSetAddAsync(
                key: $"{key}:active_dates",
                member: today,
                score: dateNumber
            );
        }

        public static async Task<uint> GetLast30DaysRunningTimeAsync(IConnectionMultiplexer connectionMultiplexer, string key)
        {
            IDatabase redisDb = connectionMultiplexer.GetDatabase();
            var now = DateTime.Now.ToLocalTime();
            var thirtyDaysAgo = now.AddDays(-30).Date;

            // 1. 从 Redis HASH 获取所有日期字段
            var allDays = await redisDb.HashGetAllAsync($"{key}:days");

            // 2. 过滤近30天的数据并累加
            uint totalSeconds = 0;
            foreach (var day in allDays)
            {
                var date = DateTime.Parse(day.Name);
                if (date >= thirtyDaysAgo)
                {
                    totalSeconds += (uint)day.Value;
                }
            }

            return totalSeconds;
        }

        public static async Task CleanupOldDataAsync(IDatabase redisDb, string key)
        {
            var thirtyDaysAgo = DateTime.Now.ToLocalTime().AddDays(-30).Date;
            int dateNumber = int.Parse(thirtyDaysAgo.Date.ToString("yyyyMMdd"));
            // 1. 从 ZSET 获取30天前的日期
            var oldDates = await redisDb.SortedSetRangeByScoreAsync(
                key: $"{key}:active_dates",
                start: 0,
                stop: dateNumber
            );

            // 2. 删除 HASH 中的旧数据
            if (oldDates.Length > 0)
            {
                await redisDb.HashDeleteAsync(
                    key: $"{key}:days",
                    hashFields: oldDates.Select(x => (RedisValue)x.ToString()).ToArray()
                );

                // 3. 从 ZSET 移除旧日期
                await redisDb.SortedSetRemoveRangeByScoreAsync(
                    key: $"{key}:active_dates",
                    start: 0,
                    stop: thirtyDaysAgo.Ticks
                );
            }
        }

        /// <summary>
        /// 将异常发布到mqtt（支持断点续传）
        /// </summary>
        /// <param name="mqttExplorer"></param>
        /// <param name="equipNotice"></param>
        /// <param name="equipId"></param>
        /// <returns></returns>
        public static async Task ExceptionPublishToMQTT(IMqttExplorer mqttExplorer, EquipNotice equipNotice, Guid equipId)
        {
            var topic = IotTopicBuilder
                .CreateIotBuilder()
                .WithPrefix(TopicType.Iot)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.Alarm)
                .WithDeviceType(EquipConnType.IotServer.ToString())
                .WithUri(equipId.ToString()!)
                .Build();

            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(equipNotice));

            // 使用支持断点续传的发布方法，异常消息优先级较高
            if (mqttExplorer is Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport.IMqttExplorerWithOffline mqttWithOffline)
            {
                await mqttWithOffline.PublishWithOfflineSupportAsync(topic, payload, priority: 1, maxRetryCount: 5);
            }
            else
            {
                await mqttExplorer.PublishAsync(topic, payload);
            }
        }

        /// <summary>
        /// 将异常记录到数据库
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        /// <param name="equipNotice"></param>
        /// <returns></returns>
        public static async Task RecordExceptionToDB(ISqlSugarClient sqlSugarClient, EquipNotice equipNotice)
        {
            // 将异常记录到数据库
            equipNotice.Id = Guid.NewGuid();
            EquipNotice sequipNotice = await sqlSugarClient.Insertable(equipNotice).ExecuteReturnEntityAsync();
        }

        /// <summary>
        /// 获取枚举的Description值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            // 获取枚举值的类型和字段信息
            Type type = value.GetType();
            string? name = Enum.GetName(type, value);
            if (name == null) return value.ToString();

            // 获取字段的DescriptionAttribute
            FieldInfo? field = type.GetField(name);
            DescriptionAttribute? attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                                               .FirstOrDefault() as DescriptionAttribute;

            // 返回Description或枚举名称
            return attribute?.Description ?? value.ToString();
        }

    }
}
