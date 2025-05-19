using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using SqlSugar;
using StackExchange.Redis;
using System.Text;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_0.ZXWL_SL_6
{
    public class XT_0_SL_6_LocalReceive : XT_0_SL_6_ReceiveBase, ILocalReceive
    {
        public XT_0_SL_6_LocalReceive(Guid equipId, ISqlSugarClient client, IConnectionMultiplexer connectionMultiplexer, IMqttExplorer mqttExplorer) : base(equipId, client, connectionMultiplexer, mqttExplorer) { }

        public async Task<Guid> Handle(byte[] msg)
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
            byte[] workStyle = new byte[6];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, 6);

            // 健康状态信息
            // 状态类型
            byte[] stateType = new byte[3];
            Buffer.BlockCopy(buffer, 28, workStyle, 0, 3);

            // 健康状态信息第0为为1表示获取到了健康状态
            List<string> exception = GetXT_0_SL_6HealthExceptionName(stateType);
            await ReceiveHelper.ExceptionRecordToLocalDB(_sqlSugarClient, _equipId, exception);

            // 本地解析不记录物理量

            return _equipId;
        }
    }
}
