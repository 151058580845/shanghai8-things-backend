using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver
{
    public class LocalReceiveDispatch : BaseReceive
    {
        public LocalReceiveDispatch(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer) : base(equipId, _client, connectionMultiplexer, mqttExplorer) { }

        public async Task Handle(byte[] msg)
        {
            string data = Encoding.UTF8.GetString(msg);
            LoggerAdapter.LogTrace(data);

            // 使用 eventData.Data 作为 buffer
            byte[] buffer = msg;

            // 仿真试验系统识别编码
            byte simuTestSysId = buffer[0];

            // 设备类型识别编码
            byte devTypeId = buffer[1];

            // 根据仿真试验系统与设备类型,通过工厂创建各自的解析类
            // 以上所有系统固定占2个字节
            LocalReceiveFactory localReceiveFactory = new LocalReceiveFactory(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer);
            ILocalReceive localReceive = localReceiveFactory.GetOrCreateLocalReceive(simuTestSysId, devTypeId);
            await localReceive.Handle(msg);
        }
    }
}
