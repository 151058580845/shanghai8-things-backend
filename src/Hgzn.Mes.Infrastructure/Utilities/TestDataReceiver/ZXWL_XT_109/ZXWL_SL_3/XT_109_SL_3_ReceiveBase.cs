using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_109.ZXWL_SL_3
{
    public class XT_109_SL_3_ReceiveBase : ReceiveBase
    {
        public Dictionary<string, int> DeviceStateTag = new Dictionary<string, int>()
        {
            // { },
        };

        public XT_109_SL_3_ReceiveBase(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer) : base(equipId, _client, connectionMultiplexer, mqttExplorer)
        {
        }

        protected string GetDevHealthExceptionName(byte ulDevHealthState)
        {
            switch (ulDevHealthState)
            {
                case 0:
                    return "正常状态";
                case 1:
                    return "软件限位";
                case 2:
                    return "超速";
                case 3:
                    return "飞车";
                case 4:
                    return "反馈异常";
                default:
                    return "";
            }
        }
    }
}
