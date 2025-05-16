using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
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
                // 0 移动设备
                case 0 when devTypeId == 5:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetXT_0_SL_5HealthExceptionName, 3, 2);
                case 0 when devTypeId == 6:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetXT_0_SL_6HealthExceptionName, 6, 3);  // *******************************************************
                case 0 when devTypeId == 7:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_7CommonHealthExceptionName, 2, 8);

                // 310 微波/毫米波复合半实物仿真系统 
                case 1 when devTypeId == 1: // *******************************************************
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, null, 10, 3);
                case 1 when devTypeId == 2:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_2CommonHealthExceptionName, 3, 3);
                case 1 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);

                // 307 微波寻的半实物仿真系统
                case 2 when devTypeId == 1:
                    return new XT_307_SL_1_LocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer);
                case 2 when devTypeId == 2:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_2CommonHealthExceptionName, 3, 3);
                case 2 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);

                // 314 射频/光学制导半实物仿真系统
                case 3 when devTypeId == 1: // ************************************************
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, null, 10, 3);
                case 3 when devTypeId == 2:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_2CommonHealthExceptionName, 3, 3);
                case 3 when devTypeId == 3:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_3CommonHealthExceptionName, 3, 2);
                case 3 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);

                // 109 紧缩场射频光学半实物仿真系统
                case 4 when devTypeId == 3:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_3CommonHealthExceptionName, 3, 2);
                case 4 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);
                case 4 when devTypeId == 7:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_7CommonHealthExceptionName, 2, 8);

                // 108 光学复合半实物仿真系统
                case 5 when devTypeId == 3:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_3CommonHealthExceptionName, 3, 2);
                case 5 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);
                case 5 when devTypeId == 7:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_7CommonHealthExceptionName, 2, 8);

                // 121 三通道控制红外制导半实物仿真系统
                case 6 when devTypeId == 3:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_3CommonHealthExceptionName, 3, 2);
                case 6 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);
                case 6 when devTypeId == 7:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_7CommonHealthExceptionName, 2, 8);

                // 202 低温环境红外制导控制半实物仿真系统
                case 6 when devTypeId == 3:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetXT_202_SL_3CommonHealthExceptionName, 3, 6);
                case 6 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);
                case 6 when devTypeId == 7:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_7CommonHealthExceptionName, 2, 8);

                // 103 机械式制导控制半实物仿真系统
                case 8 when devTypeId == 2:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_2CommonHealthExceptionName, 3, 3);
                case 8 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);

                // 119 独立回路半实物仿真系统
                case 9 when devTypeId == 2:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_2CommonHealthExceptionName, 3, 3);
                case 9 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);

                // 112 独立回路/可见光制导半实物仿真系统
                case 10 when devTypeId == 2:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_2CommonHealthExceptionName, 3, 3);
                case 10 when devTypeId == 4:
                    return new GeneralLocalReceive(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer, GetHealthExceptions.GetSL_4CommonHealthExceptionName, 6, 2);
                default:
                    return null!;
            }
        }
    }
}
