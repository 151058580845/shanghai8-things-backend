using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver
{
    public class BaseReceive
    {
        protected ISqlSugarClient _sqlSugarClient;
        protected Guid _equipId;
        protected readonly IConnectionMultiplexer _connectionMultiplexer;
        protected readonly IMqttExplorer _mqttExplorer;

        public BaseReceive(Guid equipId,
            ISqlSugarClient client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer)
        {
            _equipId = equipId;
            _sqlSugarClient = client;
            _connectionMultiplexer = connectionMultiplexer;
            _mqttExplorer = mqttExplorer;
        }

        protected float[] GetPhysicalQuantity(byte[] buffer, int startIndex, out uint ulPhysicalQuantityCount)
        {
            byte[] physicalQuantityCount = new byte[4];
            Buffer.BlockCopy(buffer, startIndex, physicalQuantityCount, 0, 4);
            ulPhysicalQuantityCount = BitConverter.ToUInt32(physicalQuantityCount, 0);
            ulPhysicalQuantityCount -= 1; // 减去1是因为最后一个物理量是运行时间,这个是后加的,且属性类型是uint,不好转成float一起赋值,在赋值完普通物理量后我会单独赋值

            // 剩余的都给物理量
            byte[] acquData = new byte[ulPhysicalQuantityCount * 4];
            Buffer.BlockCopy(buffer, startIndex + 4, acquData, 0, acquData.Length);

            // 将 byte[] 转换为 float[] , 每个 float 占用 4 字节
            int floatCount = acquData.Length / 4;
            float[] floatData = new float[floatCount];
            for (int i = 0; i < floatCount; i++)
            {
                floatData[i] = BitConverter.ToSingle(acquData, i * 4);
            }
            return floatData;
        }
    }
}
