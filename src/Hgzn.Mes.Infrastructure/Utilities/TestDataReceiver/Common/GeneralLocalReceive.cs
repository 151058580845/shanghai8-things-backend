using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common
{
    public class GeneralLocalReceive : BaseReceive, ILocalReceive
    {
        private Func<byte[], List<string>> _getHealthException;
        private const int _WORKSTYLEANALYSISLENGTH = 10;
        private const int _STATETYPEANALYSISLENGTH = 9;
        private int _workStyleLength;
        private int _stateTypeLength;


        public GeneralLocalReceive(
            Guid equipId,
            ISqlSugarClient client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer,
            Func<byte[], List<string>> getHealthException, int workStyleLength, int stateTypeLength) : base(equipId, client, connectionMultiplexer, mqttExplorer)
        {
            this._getHealthException = getHealthException;
            this._workStyleLength = workStyleLength;
            this._stateTypeLength = stateTypeLength;
        }

        public async Task<Guid> Handle(byte[] msg, DateTime sendTime)
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
            byte[] workStyle = new byte[_WORKSTYLEANALYSISLENGTH];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, _WORKSTYLEANALYSISLENGTH);

            // 健康状态信息
            // 状态类型
            byte[] stateType = new byte[_STATETYPEANALYSISLENGTH];
            Buffer.BlockCopy(buffer, 22 + _WORKSTYLEANALYSISLENGTH, stateType, 0, _STATETYPEANALYSISLENGTH);

            // 健康状态信息第0为为1表示获取到了健康状态
            List<string> exception = _getHealthException(stateType);
            if (exception.Count > 0)
                await ReceiveHelper.ExceptionRecordToLocalDB(_sqlSugarClient, _equipId, exception);

            // 本地解析不记录物理量

            return _equipId;
        }
    }
}
