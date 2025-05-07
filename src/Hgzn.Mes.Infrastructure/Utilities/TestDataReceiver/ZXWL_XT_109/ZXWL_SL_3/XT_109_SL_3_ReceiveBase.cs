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
    public class XT_109_SL_3_ReceiveBase
    {
        public Dictionary<string, int> DeviceStateTag = new Dictionary<string, int>()
        {
            // { },
        };

        protected ISqlSugarClient SqlSugarClient;
        protected Guid _equipId;
        protected readonly IConnectionMultiplexer _connectionMultiplexer;
        protected readonly IMqttExplorer _mqttExplorer;

        public XT_109_SL_3_ReceiveBase(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer)
        {
            _equipId = equipId;
            SqlSugarClient = _client;
            _connectionMultiplexer = connectionMultiplexer;
            _mqttExplorer = mqttExplorer;
        }

        protected string GetDevHealthExceptionName(byte ulDevHealthState)
        {
            switch (ulDevHealthState)
            {
                case 0:
                    return "软件限位";
                case 1:
                    return "超速";
                case 2:
                    return "飞车";
                case 3:
                    return "反馈异常";
                default:
                    return "";
            }
        }
    }
}
