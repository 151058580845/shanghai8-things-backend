using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_314.ZXWL_SL_1
{
    public class XT_314_SL_1_ReceiveBase : BaseReceive
    {
        protected Dictionary<string, int> DeviceStateTag = new Dictionary<string, int>()
        {
            // { },
        };

        public XT_314_SL_1_ReceiveBase(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer) : base(equipId, _client, connectionMultiplexer, mqttExplorer)
        {
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
        protected readonly Dictionary<(Checkout, SupplyVoltageState), string> SupplyVoltageExceptionMessages = new()
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
        protected readonly Dictionary<Checkout, string> DevHealthExceptionMessages = new()
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

        protected string? CheckChannelsAndZones(uint ulState)
        {
            string rst = null;
            // 检查垂直通道和水平通道
            for (int i = 0; i < 8; i++)
            {
                if ((ulState & 1 << 8 + i) != 0) // D8~D15
                    rst = $"垂直通道{i + 1}（含预留通道）";
                if ((ulState & 1 << 16 + i) != 0) // D16~D2
                    rst = $"水平通道{i + 1}（含预留通道）";
            }

            // 检查分区
            for (int i = 0; i < 8; i++)
            {
                if ((ulState & 1 << 24 + i) != 0) // D24~D31
                    rst += $"分区{i + 1}";
            }

            return rst; // 没有找到通道或分区异常
        }
    }
}
