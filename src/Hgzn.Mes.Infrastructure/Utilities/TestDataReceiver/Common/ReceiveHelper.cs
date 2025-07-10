using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Newtonsoft.Json;
using SqlSugar;
using StackExchange.Redis;
using System;
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
        public static DbConnOptions LOCALDBCONFIG = new DbConnOptions
        {
            DbType = DbType.OpenGauss,
            Url = "PORT=5432;DATABASE=opengauss;HOST=localhost;PASSWORD=Dev@2024;USER ID=dev;No Reset On Close=true",
            EnabledReadWrite = false,
            EnabledCodeFirst = true,
            EnabledSqlLog = true,
            EnabledDbSeed = true,
            EnabledSaasMultiTenancy = false
        };
        private const int BodyStartIndex = 13;
        public static bool GetMessage(byte[] buffer, long size, out byte[] message)
        {
            //标准头
            var header = buffer[0];
            if (header != 0x5A)
            {
                LoggerAdapter.LogWarning("报头符错误。");
                message = null!;
                return false;
            }

            //报文长度
            var messageLength = BitConverter.ToUInt32(buffer, 1);
            //if (messageLength != size)
            //{
            //    LoggerAdapter.LogWarning($"报文长度错误：接收到的长度为 {size}，报文中声明的长度为 {messageLength}");
            //    message = null!;
            //    return false;
            //}

            //解析报文流水号（1字节）
            byte number = buffer[5];

            // 解析时间
            byte[] bYear = new byte[2];
            Buffer.BlockCopy(buffer, 6, bYear, 0, 2);
            ushort year = BitConverter.ToUInt16(bYear, 0);

            byte month = buffer[8];
            byte day = buffer[9];
            byte hour = buffer[10];
            byte minute = buffer[11];
            byte second = buffer[12];

            //报文数据
            var length = messageLength - 13;
            message = new byte[length];
            Buffer.BlockCopy(buffer, BodyStartIndex, message, 0, message.Length);
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
        /// 将异常记录到redis
        /// </summary>
        /// <param name="connectionMultiplexer"></param>
        /// <param name="simuTestSysId"></param>
        /// <param name="devTypeId"></param>
        /// <param name="compId"></param>
        /// <param name="equipId"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task<EquipNotice> ExceptionRecordToRedis(IConnectionMultiplexer connectionMultiplexer, byte simuTestSysId, byte devTypeId, byte[] compId, Guid equipId, List<string> exception)
        {
            // 记录到redis
            IDatabase redisDb = connectionMultiplexer.GetDatabase();
            var key = string.Format(CacheKeyFormatter.EquipHealthStatus, simuTestSysId, devTypeId, compId);
            await redisDb.StringSetAsync(key, exception.Count);
            EquipNotice equipNotice = new EquipNotice()
            {
                EquipId = equipId,
                SendTime = DateTime.Now.ToLocalTime(),
                NoticeType = EquipNoticeType.Alarm,
                Title = "Receive Alarm",
                Content = JsonConvert.SerializeObject(exception),
                Description = "",
            };
            return equipNotice;
        }

        /// <summary>
        /// 将异常记录到redis
        /// </summary>
        /// <param name="connectionMultiplexer"></param>
        /// <param name="simuTestSysId"></param>
        /// <param name="devTypeId"></param>
        /// <param name="compId"></param>
        /// <param name="equipId"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task<EquipNotice> ExceptionRecordToRedis(IConnectionMultiplexer connectionMultiplexer, byte simuTestSysId, byte devTypeId, byte[] compId, Guid equipId, string exception)
        {
            // 记录到redis
            IDatabase redisDb = connectionMultiplexer.GetDatabase();
            var key = string.Format(CacheKeyFormatter.EquipHealthStatus, simuTestSysId, devTypeId, compId);
            await redisDb.StringSetAsync(key, 1);
            EquipNotice equipNotice = new EquipNotice()
            {
                EquipId = equipId,
                SendTime = DateTime.Now.ToLocalTime(),
                NoticeType = EquipNoticeType.Alarm,
                Title = "Receive Alarm",
                Content = JsonConvert.SerializeObject(exception),
                Description = "",
            };
            return equipNotice;
        }

        /// <summary>
        /// 将异常发布到mqtt
        /// </summary>
        /// <param name="mqttExplorer"></param>
        /// <param name="equipNotice"></param>
        /// <param name="equipId"></param>
        /// <returns></returns>
        public static async Task ExceptionPublishToMQTT(IMqttExplorer mqttExplorer, EquipNotice equipNotice, Guid equipId)
        {
            // 将异常发布到mqtt
            await mqttExplorer.PublishAsync(IotTopicBuilder
            .CreateIotBuilder()
            .WithPrefix(TopicType.Iot)
            .WithDirection(MqttDirection.Up)
            .WithTag(MqttTag.Alarm)
            .WithDeviceType(EquipConnType.IotServer.ToString())
            .WithUri(equipId.ToString()!)
            .Build(), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(equipNotice)));
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
