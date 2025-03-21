using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Equip.EquipData;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared;
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

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver
{
    public class TestDataLocalReceive : ReceiveBase
    {
        public Dictionary<string, int> DeviceStateTag = new Dictionary<string, int>()
        {
            // { },
        };

        public TestDataLocalReceive(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer) : base(equipId, _client, connectionMultiplexer, mqttExplorer) { }

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
        public async Task<string> Handle(byte[] msg)
        {
            string data = Encoding.UTF8.GetString(msg);
            LoggerAdapter.LogTrace(data);

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

            // 这里根据设备的不同有些设备只有电源电压参数,所以去获取电源电压异常,有些设备只有自检状态参数,所以去获取自检状态异常
            // 这里定义如果该设备的值是0,则检查自检状态异常,如果是1则检查电源电压异常
            List<string> exception = new List<string>();
            if (DeviceStateTag.ContainsKey(compNumber))
            {
                if (DeviceStateTag[compNumber] == 0)
                    // 获取自检状态异常
                    exception = GetDevHealthExceptionName(ulDevHealthState);
                else if (DeviceStateTag[compNumber] == 1)
                    // 获取电源电压异常
                    exception = GetSupplyVoltageExceptionName(ulSupplyVoltageState);
            }

            EquipNotice equipNotice = new EquipNotice()
            {
                EquipId = _equipId,
                SendTime = DateTime.Now,
                NoticeType = EquipNoticeType.Alarm,
                Title = "Receive Alarm",
                Content = JsonConvert.SerializeObject(exception),
                Description = "",
            };

            // 将异常记录到数据库
            equipNotice.Id = Guid.NewGuid();
            EquipNotice sequipNotice = await SqlSugarClient.Insertable(equipNotice).ExecuteReturnEntityAsync();

            return compNumber;
        }
    }
}
