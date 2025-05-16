using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_0_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_0.ZXWL_SL_5
{
    public class XT_0_SL_5_OnlineReceive : XT_0_SL_5_ReceiveBase, ILocalReceive
    {
        public XT_0_SL_5_OnlineReceive(Guid equipId, ISqlSugarClient client, IConnectionMultiplexer connectionMultiplexer, IMqttExplorer mqttExplorer) : base(equipId, client, connectionMultiplexer, mqttExplorer) { }

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
            byte[] workStyle = new byte[3];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, 3);


            // *** 健康状态信息
            // 状态类型
            byte[] stateType = new byte[2];
            Buffer.BlockCopy(buffer, 25, workStyle, 0, 2);


            // *** 物理量
            // 剩余的都给物理量
            uint ulPhysicalQuantityCount;
            float[] floatData = GetPhysicalQuantity(buffer, 27, out ulPhysicalQuantityCount);


            // *** 构建entity
            XT_0_SL_5_ReceiveDatas entity = new XT_0_SL_5_ReceiveDatas()
            {
                Id = _equipId,
                CreationTime = DateTime.Now,
                SimuTestSysld = simuTestSysId,
                DevTypeld = devTypeId,
                Compld = compNumber,
                LocalOrRemote = workStyle[0],
                PowerSupplyType = workStyle[1],
                Reserved = workStyle[2],
                StatusType = stateType[0],
                OperationStatus = stateType[1],
                PhysicalParameterCount = ulPhysicalQuantityCount
            };
            // 使用反射将后面指定数量个物理量数据进行填充
            PropertyInfo[] properties = typeof(XT_0_SL_5_ReceiveDatas).GetProperties();
            // 使用循环将数组值赋给类属性
            for (int i = 0; i < floatData.Length; i++)
            {
                // 跳过前面11个属性
                properties[i + 11].SetValue(entity, floatData[i]);
            }


            // *** 处理异常信息
            List<string> exception = GetSL_5HealthExceptionName(stateType);
            // 将试验数据记录数据库
            XT_0_SL_5_ReceiveDatas receive = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            // 将试验数据的数据部分推送到mqtt给前端进行展示(暂时不进行数据展示)
            // await TestDataPublishToMQTT(receive);
            // 将异常记录到redis
            EquipNotice equipNotice = await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, simuTestSysId, devTypeId, compId, _equipId, exception);
            // 将异常发布到mqtt
            await ReceiveHelper.ExceptionPublishToMQTT(_mqttExplorer, equipNotice, _equipId);
            // 将异常记录到数据库
            await ReceiveHelper.RecordExceptionToDB(_sqlSugarClient, equipNotice);

            return _equipId;
        }
    }
}
