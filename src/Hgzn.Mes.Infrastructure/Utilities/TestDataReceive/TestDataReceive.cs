using Hgzn.Mes.Domain.Entities.System.Equip.EquipData;
using Hgzn.Mes.Domain.Events;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared;
using MediatR;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Newtonsoft.Json;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Domain.ValueObjects.Message;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceive
{
    public class TestDataReceive
    {
        public Dictionary<string, int> DeviceStateTag = new Dictionary<string, int>()
        {
            // { },
        };
        public ISqlSugarClient SqlSugarClient;
        private Guid _equipId;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IMqttExplorer _mqttExplorer;

        public TestDataReceive(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer)
        {
            this._equipId = equipId;
            this.SqlSugarClient = _client;
            this._connectionMultiplexer = connectionMultiplexer;
            this._mqttExplorer = mqttExplorer;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="eventData"></param>
        /// simuTestSysId   字节1     固定
        /// devTypeId       字节1     固定
        /// compId          字节20    固定
        /// workStyle       字节10    不固定
        /// devHealthState   字节8     不固定
        /// acquData        字节2800  不固定
        public async Task<Guid> Handle(byte[] msg, bool needPublish)
        {
            string data = Encoding.UTF8.GetString(msg);
            Console.WriteLine(data);

            // 使用 eventData.Data 作为 buffer
            byte[] buffer = msg;

            // 仿真试验系统识别编码
            byte simuTestSysId = buffer[0];

            // 设备类型识别编码
            byte devTypeId = buffer[1];

            // 本机识别编码
            byte[] compId = new byte[20];
            Buffer.BlockCopy(buffer, 2, compId, 0, 20);
            string compNumber = Encoding.ASCII.GetString(compId).Trim('\0');

            // 工作模式信息
            byte[] workStyle = new byte[10];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, 10);

            // 健康状态信息
            // 状态类型
            byte stateType = buffer[32];

            // 自检状态1个uint,表中是4位的ulong,在C#中,直接用uint代替
            byte[] devHealthState = new byte[4];
            Buffer.BlockCopy(buffer, 33, devHealthState, 0, 4);
            uint ulDevHealthState = BitConverter.ToUInt32(devHealthState, 0);

            // 电源电压状态1个uint,表中是4位的ulong,在C#中,直接用uint代替
            byte[] supplyVoltageState = new byte[4];
            Buffer.BlockCopy(buffer, 37, devHealthState, 0, 4);
            uint ulSupplyVoltageState = BitConverter.ToUInt32(supplyVoltageState, 0);

            // 物理量数量
            byte[] physicalQuantityCount = new byte[4];
            Buffer.BlockCopy(buffer, 41, physicalQuantityCount, 0, 4);
            uint ulPhysicalQuantityCount = BitConverter.ToUInt32(physicalQuantityCount, 0);

            // 剩余的都给物理量
            byte[] acquData = new byte[ulPhysicalQuantityCount * 4];
            Buffer.BlockCopy(buffer, 45, acquData, 0, acquData.Length);

            // 将 byte[] 转换为 float[] , 每个 float 占用 4 字节
            int floatCount = acquData.Length / 4;
            float[] floatData = new float[floatCount];
            for (int i = 0; i < floatCount; i++)
            {
                floatData[i] = BitConverter.ToSingle(acquData, i * 4);
            }

            ReceiveData entity = new ReceiveData()
            {
                Id = _equipId,
                CreationTime = DateTime.Now,
                SimuTestSysld = simuTestSysId,
                DevTypeld = devTypeId,
                Compld = compNumber,
                MicroWare = workStyle[0],
                Channel = workStyle[1],
                ModelValid = workStyle[2],
                Model1 = workStyle[3],
                Model2 = workStyle[4],
                Model3 = workStyle[5],
                Model4 = workStyle[6],
                Model5 = workStyle[7],
                Model6 = workStyle[8],
                Model7 = workStyle[9],
                StateType = stateType,
                SelfTest = ulDevHealthState,
                SupplyVoltageState = ulSupplyVoltageState,
                PhysicalQuantityCount = ulPhysicalQuantityCount
            };

            // 使用反射将后面指定数量个物理量数据进行填充
            System.Reflection.PropertyInfo[] properties = typeof(ReceiveData).GetProperties();

            // 使用循环将数组值赋给类属性
            for (int i = 0; i < floatData.Length; i++)
            {
                // 跳过前面20个属性,索引20开始设置
                properties[i + 20].SetValue(entity, floatData[i]);
            }

            // 这里根据设备的不同有些设备只有电源电压参数,所以去获取电源电压异常,有些设备只有自检状态参数,所以去获取自检状态异常
            // 这里定义如果该设备的值是0,则检查自检状态异常,如果是1则检查电源电压异常
            List<string> exception = new List<string>();
            if (DeviceStateTag.ContainsKey(compNumber))
            {
                if (DeviceStateTag[compNumber] == 0)
                    // 获取自检状态异常
                    exception = GetDevHealthExceptionName(ulSupplyVoltageState);
                else if (DeviceStateTag[compNumber] == 1)
                    // 获取电源电压异常
                    exception = GetSupplyVoltageExceptionName(ulSupplyVoltageState);
            }

            // ReceiveData receive = await SqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();

            // 记录到redis
            IDatabase redisDb = _connectionMultiplexer.GetDatabase();
            var key = string.Format(CacheKeyFormatter.EquipHealthStatus, simuTestSysId, devTypeId, compId);
            await redisDb.StringSetAsync(key, exception.Count);
            EquipNotice equipNotice = new EquipNotice()
            {
                EquipId = _equipId,
                SendTime = DateTime.Now,
                NoticeType = EquipNoticeType.Alarm,
                Title = "Receive Alarm",
                Content = JsonConvert.SerializeObject(exception),
                Description = "",
            };

            if (needPublish)
            {
                // 将异常发布到mqtt
                await _mqttExplorer.PublishAsync(IotTopicBuilder
                .CreateIotBuilder()
                .WithPrefix(TopicType.Iot)
                .WithDirection(MqttDirection.Up)
                .WithTag(MqttTag.Alarm)
                .WithDeviceType(EquipConnType.IotServer.ToString())
                .WithUri(_equipId.ToString()!)
                .Build(), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(equipNotice)));
            }

            // 将异常记录到数据库
            equipNotice.Id = Guid.NewGuid();
            EquipNotice sequipNotice = await SqlSugarClient.Insertable(equipNotice).ExecuteReturnEntityAsync();
            
            // return receive.Id;
            return Guid.Empty;
        }

        // 定义状态位的枚举
        [Flags]
        public enum Checkout : uint
        {
            None = 0,
            D0 = 1 << 0,
            D1 = 1 << 1,
            D2 = 1 << 2,
            D3 = 1 << 3,
        }

        [Flags]
        public enum SupplyVoltageState : uint
        {
            None = 0,
            D0 = 1 << 0,
            D1 = 1 << 1,
            D2 = 1 << 2,
            D3 = 1 << 3,
            D4 = 1 << 4,
            D5 = 1 << 5,
            D6 = 1 << 6,
            D7 = 1 << 7,
            D8 = 1 << 8,
            D16 = 1 << 16,
            D24 = 1 << 24,
        }

        [Flags]
        public enum DevHealthState : uint
        {
            None = 0,
            D8 = 1 << 8,
            D16 = 1 << 16,
            D24 = 1 << 24,
        }

        // 定义异常信息
        private readonly Dictionary<(Checkout, SupplyVoltageState), string> SupplyVoltageExceptionMessages = new()
        {
            { (Checkout.D0, SupplyVoltageState.D4), "解析器件5V/12V-12V电压异常" },
            { (Checkout.D0, SupplyVoltageState.D5), "解析放大器15V电压异常" },
            { (Checkout.D0, SupplyVoltageState.D6), "解析控制12V电压异常" },
            { (Checkout.D0, SupplyVoltageState.D7), "精控解析风扇12V电压异常" },
            { (Checkout.D1, SupplyVoltageState.D4), "精控器件5V/12V-12V电压异常" },
            { (Checkout.D1, SupplyVoltageState.D5), "精控放大器15V电压异常" },
            { (Checkout.D1, SupplyVoltageState.D6), "精控控制12V电压异常" },
            { (Checkout.D1, SupplyVoltageState.D7), "放大器风扇12V电压异常" },
            { (Checkout.D2, SupplyVoltageState.D4), "粗控器件5V/12V-12V电压异常" },
            { (Checkout.D2, SupplyVoltageState.D6), "粗控控制12V电压异常" },
            { (Checkout.D2, SupplyVoltageState.D7), "粗控风扇12V电压异常" },
            { (Checkout.D3, SupplyVoltageState.D4), "管理器件15V电压异常" },
            { (Checkout.D3, SupplyVoltageState.D6), "管理控制12V电压异常" },
            { (Checkout.D3, SupplyVoltageState.D7), "管理风扇12V电压异常" },
        };

        // 定义异常信息
        private readonly Dictionary<Checkout, string> DevHealthExceptionMessages = new()
        {
            { Checkout.D0, "解析自检异常" },
            { Checkout.D1, "精控自检异常" },
            { Checkout.D2, "粗控自检异常" },
            { Checkout.D3, "阵列管理自检异常" },
        };

        public List<string> GetSupplyVoltageExceptionName(uint ulSupplyVoltageState)
        {
            List<string> rst = new List<string>();
            // 检查 ulSupplyVoltageState 的状态
            IEnumerable<KeyValuePair<(Checkout, SupplyVoltageState), string>> exception = SupplyVoltageExceptionMessages
                .Where(entry =>
                    (ulSupplyVoltageState & (uint)entry.Key.Item1) != 0 &&
                    (ulSupplyVoltageState & (uint)entry.Key.Item2) != 0);

            List<string> exceptionNames = new List<string>();
            foreach (KeyValuePair<(Checkout, SupplyVoltageState), string> item in exception)
            {
                exceptionNames.Add(item.Value);
            }
            // 检查通道和分区
            string? channelAndZone = CheckChannelsAndZones(ulSupplyVoltageState);
            if (channelAndZone != null)
            {
                foreach (string exceptionName in exceptionNames)
                {
                    rst.Add(channelAndZone + exceptionName);
                }
            }
            return rst; // 如果没有异常，则表示正常
        }

        public List<string> GetDevHealthExceptionName(uint ulDevHealthState)
        {
            List<string> rst = new List<string>();
            // 检查 ulDevHealthState 的状态
            IEnumerable<KeyValuePair<Checkout, string>> exception = DevHealthExceptionMessages
                .Where(entry => (ulDevHealthState & (uint)entry.Key) != 0);
            List<string> exceptionNames = new List<string>();
            foreach (KeyValuePair<Checkout, string> item in exception)
            {
                exceptionNames.Add(item.Value);
            }
            // 检查通道和分区
            string? channelAndZone = CheckChannelsAndZones(ulDevHealthState);
            if (channelAndZone != null)
            {
                foreach (string exceptionName in exceptionNames)
                {
                    rst.Add(channelAndZone + exceptionName);
                }
            }
            return rst; // 如果没有异常，则表示正常
        }

        private string? CheckChannelsAndZones(uint ulState)
        {
            string rst = null;
            // 检查垂直通道和水平通道
            for (int i = 0; i < 8; i++)
            {
                if ((ulState & (1 << (8 + i))) != 0) // D8~D15
                    rst = $"垂直通道{i + 1}（含预留通道）";
                if ((ulState & (1 << (16 + i))) != 0) // D16~D2
                    rst = $"水平通道{i + 1}（含预留通道）";
            }

            // 检查分区
            for (int i = 0; i < 8; i++)
            {
                if ((ulState & (1 << (24 + i))) != 0) // D24~D31
                    rst += $"分区{i + 1}";
            }

            return rst; // 没有找到通道或分区异常
        }
    }
}