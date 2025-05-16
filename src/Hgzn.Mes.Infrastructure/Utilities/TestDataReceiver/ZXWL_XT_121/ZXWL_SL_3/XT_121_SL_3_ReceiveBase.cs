using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_121.ZXWL_SL_3
{
    public class XT_121_SL_3_ReceiveBase : ReceiveBase
    {
        public Dictionary<string, int> DeviceStateTag = new Dictionary<string, int>()
        {
            // { },
        };

        public XT_121_SL_3_ReceiveBase(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer) : base(equipId, _client, connectionMultiplexer, mqttExplorer)
        {
        }
    }
}
