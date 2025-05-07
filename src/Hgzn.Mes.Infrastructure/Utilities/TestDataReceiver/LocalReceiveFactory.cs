using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_109.ZXWL_SL_3;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_121.ZXWL_SL_3;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_307.ZXWL_SL_1;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver
{
    public class LocalReceiveFactory
    {
        protected ISqlSugarClient _sqlSugarClient;
        protected Guid _equipId;
        protected readonly IConnectionMultiplexer _connectionMultiplexer;
        protected readonly IMqttExplorer _mqttExplorer;

        public LocalReceiveFactory(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer)
        {
            _equipId = equipId;
            _sqlSugarClient = _client;
            _connectionMultiplexer = connectionMultiplexer;
            _mqttExplorer = mqttExplorer;
        }

        public ILocalReceive CreateLocalReceive(byte simuTestSysId, byte devTypeId)
        {
            switch (simuTestSysId)
            {
                case 4 when devTypeId == 3:
                    return new XT_109_SL_3_LocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer);
                case 6 when devTypeId == 3:
                    return new XT_121_SL_3_LocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer);
                case 2 when devTypeId == 1:
                    return new XT_307_SL_1_LocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer);
                default:
                    return null!;
            }
        }
    }
}
