using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_109.ZXWL_SL_3
{
    internal class XT_109_SL_3_LocalReceive : BaseReceive, ILocalReceive
    {
        private Func<short[], List<string>> _getHealthException;
        private const int _WORKSTYLEANALYSISLENGTH = 10;
        private const int _STATETYPEANALYSISLENGTH = 9;
        private int _workStyleLength;
        private int _stateTypeLength;

        public XT_109_SL_3_LocalReceive(
            Guid equipId,
            ISqlSugarClient client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer,
            Func<short[], List<string>> getHealthException, int workStyleLength, int stateTypeLength) :
            base(equipId, client, connectionMultiplexer, mqttExplorer)
        {
            this._getHealthException = getHealthException;
            this._workStyleLength = workStyleLength;
            this._stateTypeLength = stateTypeLength;
        }

        public async Task<Guid> Handle(byte[] msg, DateTime time)
        {
            string data = Encoding.UTF8.GetString(msg);

            // 使用 eventData.Data 作为 buffer
            byte[] buffer = msg;
            // 仿真试验系统识别编码
            byte simuTestSysId = buffer[0];
            LoggerAdapter.LogDebug($"AG - 本地解析 - 仿真试验系统识别编码:{simuTestSysId}");
            // 设备类型识别编码
            byte devTypeId = buffer[1];
            LoggerAdapter.LogDebug($"AG - 本地解析 - 设备类型识别编码:{devTypeId}");
            // 本机识别编码
            byte[] compId = new byte[20];
            Buffer.BlockCopy(buffer, 2, compId, 0, 20);
            string compNumber = Encoding.ASCII.GetString(compId).Trim('\0');
            // 最新需求,记录数据库的时候去掉引号保存
            compNumber = compNumber.Trim('"');
            LoggerAdapter.LogDebug($"AG - 本地解析 - 本机识别编码:{compNumber}");

            // 工作模式信息
            byte[] workStyle = new byte[_WORKSTYLEANALYSISLENGTH];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, _WORKSTYLEANALYSISLENGTH);
            LoggerAdapter.LogDebug($"AG - 本地解析 - 工作模式信息:{string.Join(", ", workStyle.Select(x => (int)x))}");

            // *** 健康状态信息
            // 状态类型
            byte statusType = buffer[22 + _WORKSTYLEANALYSISLENGTH];
            LoggerAdapter.LogDebug($"AG - 本地解析 - 状态类型(是否获取到健康状态):{statusType}");
            short[] healthStatus = new short[5];
            Buffer.BlockCopy(buffer, 22 + _WORKSTYLEANALYSISLENGTH + 1, healthStatus, 0, 10);
            LoggerAdapter.LogDebug($"AG - 本地解析 - 健康状态信息:{string.Join(", ", healthStatus.Select(x => (int)x))}");

            // *** 物理量
            // 剩余的都给物理量
            uint ulPhysicalQuantityCount;
            float[] floatData = GetPhysicalQuantity(buffer, 22 + _WORKSTYLEANALYSISLENGTH + _STATETYPEANALYSISLENGTH, out ulPhysicalQuantityCount);
            LoggerAdapter.LogDebug($"AG - 本地解析 - 物理量数量(不包含运行时长):{ulPhysicalQuantityCount}");

            // *** 运行时间
            // 计算需要拷贝的起始位置和需要的总长度
            int startPosition = (int)((ulPhysicalQuantityCount + 1) * 4 + 22 + _WORKSTYLEANALYSISLENGTH + _STATETYPEANALYSISLENGTH);
            int requiredLength = startPosition + 4;
            uint ulRunTime = 0;
            if (buffer.Length >= requiredLength)
            {
                byte[] runTime = new byte[4];
                Buffer.BlockCopy(buffer, startPosition, runTime, 0, 4);
                ulRunTime = BitConverter.ToUInt32(runTime, 0);
            }
            LoggerAdapter.LogDebug($"AG - 本地解析 - 运行时长:{ulRunTime}");

            // *** 处理异常信息
            List<string> exception = _getHealthException(healthStatus);
            LoggerAdapter.LogDebug($"AG - 远程解析 - 健康检查异常列表（共 {exception.Count} 条）:\n{string.Join("\n", exception)}");

            return _equipId;
        }
    }
}
