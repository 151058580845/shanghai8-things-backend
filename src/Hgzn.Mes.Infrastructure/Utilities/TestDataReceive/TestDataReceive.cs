using Hgzn.Mes.Domain.Entities.System.Equip.EquipData;
using Hgzn.Mes.Domain.Events;
using MediatR;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceive
{
    public class TestDataReceive
    {
        public ISqlSugarClient SqlSugarClient;

        public TestDataReceive(ISqlSugarClient _client)
        {
            this.SqlSugarClient = _client;
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
        public async Task Handle(byte[] msg)
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
            // 获取电源电压字符串
            string sSupplyVoltageState = GetExceptionName(ulDevHealthState, ulSupplyVoltageState);

            // 剩余的都给物理量 48 + 16 = 64
            byte[] acquData = new byte[buffer.Length - 64];
            Buffer.BlockCopy(buffer, 41, acquData, 0, acquData.Length);

            // 将 byte[] 转换为 float[] , 每个 float 占用 4 字节
            int floatCount = acquData.Length / 4;
            float[] floatData = new float[floatCount];
            for (int i = 0; i < floatCount; i++)
            {
                floatData[i] = BitConverter.ToSingle(acquData, i * 4);
            }

            ReceiveData entity = new ReceiveData()
            {
                Id = Guid.NewGuid(),
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
                StateType = stateType,
                SelfTest = ulDevHealthState,
                SupplyVoltageState = sSupplyVoltageState
            };

            // 使用反射将后面600个物理量数据进行填充
            System.Reflection.PropertyInfo[] properties = typeof(ReceiveData).GetProperties();
            // 要求物理量数量与属性数量一致
            if (floatCount == 462)
            {
                // 使用循环将数组值赋给类属性
                for (int i = 0; i < floatData.Length; i++)
                {
                    // 跳过前面16个属性,索引16开始设置
                    properties[i + 16].SetValue(entity, floatData[i]);
                }
            }

            SqlSugarClient.Insertable(entity);
        }

        // 定义状态位的枚举
        [Flags]
        public enum DevHealthState : uint
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

        // 定义异常信息
        private readonly Dictionary<(DevHealthState, SupplyVoltageState), string> ExceptionMessages = new()
    {
        {(DevHealthState.D0, SupplyVoltageState.D4), "解析器件5V/12V-12V电压异常"},
        {(DevHealthState.D0, SupplyVoltageState.D5), "解析放大器15V电压异常"},
        {(DevHealthState.D0, SupplyVoltageState.D6), "解析控制12V电压异常"},
        {(DevHealthState.D0, SupplyVoltageState.D7), "解析风扇12V电压异常"},
        {(DevHealthState.D1, SupplyVoltageState.D4), "精控器件5V/12V-12V电压异常"},
        {(DevHealthState.D1, SupplyVoltageState.D5), "精控放大器15V电压异常"},
        {(DevHealthState.D1, SupplyVoltageState.D6), "精控控制12V电压异常"},
        {(DevHealthState.D1, SupplyVoltageState.D7), "精控风扇12V电压异常"},
        {(DevHealthState.D2, SupplyVoltageState.D4), "粗控器件5V/12V-12V电压异常"},
        {(DevHealthState.D2, SupplyVoltageState.D5), "粗控控制12V电压异常"},
        {(DevHealthState.D2, SupplyVoltageState.D6), "粗控风扇12V电压异常"},
        {(DevHealthState.D3, SupplyVoltageState.D4), "管理器件15V电压异常"},
        {(DevHealthState.D3, SupplyVoltageState.D5), "管理控制12V电压异常"},
        {(DevHealthState.D3, SupplyVoltageState.D6), "管理风扇12V电压异常"},
    };

        public string GetExceptionName(uint ulDevHealthState, uint ulSupplyVoltageState)
        {
            // 检查 ulSupplyVoltageState 的状态
            KeyValuePair<(DevHealthState, SupplyVoltageState), string> exception = ExceptionMessages
                .FirstOrDefault(entry => (ulDevHealthState & (uint)entry.Key.Item1) != 0 && (ulSupplyVoltageState & (uint)entry.Key.Item2) != 0);
            if (exception.Value != null)
                return exception.Value;
            // 检查通道和分区
            string? channelOrZone = CheckChannelsAndZones(ulSupplyVoltageState);
            if (channelOrZone != null)
                return channelOrZone;
            return "正常"; // 如果没有异常，返回正常
        }

        private string? CheckChannelsAndZones(uint ulSupplyVoltageState)
        {
            // 检查垂直通道和水平通道
            for (int i = 0; i < 8; i++)
            {
                if ((ulSupplyVoltageState & (1 << (8 + i))) != 0) // D8~D15
                    return $"垂直通道{i + 1}（含预留通道）";
                if ((ulSupplyVoltageState & (1 << (16 + i))) != 0) // D16~D2
                    return $"水平通道{i + 1}（含预留通道）";
            }

            // 检查分区
            for (int i = 0; i < 8; i++)
            {
                if ((ulSupplyVoltageState & (1 << (24 + i))) != 0) // D24~D31
                    return $"分区{i + 1}（仅粗控、阵列管理有效）";
            }
            return null; // 没有找到通道或分区异常
        }
    }
}
