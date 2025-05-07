using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_109_ReceiveDatas;
using Newtonsoft.Json;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using System.Reflection;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_109.ZXWL_SL_3
{
    public class XT_109_SL_3_OnlineReceive : XT_109_SL_3_ReceiveBase, IOnlineReceive
    {
        public XT_109_SL_3_OnlineReceive(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer) : base(equipId, _client, connectionMultiplexer, mqttExplorer)
        {

        }

        /// <summary>
        /// 解析数据
        /// </summary>
        public async Task<Guid> Handle(byte[] msg)
        {
            string data = Encoding.UTF8.GetString(msg);
            LoggerAdapter.LogTrace(data);

            #region 组织实体,记录与展示

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

            // 健康状态信息
            // 状态类型
            byte stateType = buffer[25];

            // 物理量数量
            byte[] physicalQuantityCount = new byte[4];
            Buffer.BlockCopy(buffer, 41, physicalQuantityCount, 0, 4);
            uint ulPhysicalQuantityCount = BitConverter.ToUInt32(physicalQuantityCount, 0);

            // 剩余的都给物理量
            byte[] acquData = new byte[ulPhysicalQuantityCount * 4];
            Buffer.BlockCopy(buffer, 45, acquData, 0, acquData.Length);

            // 将 byte[] 转换为 float[] , 每个 float 占用 4 字节
            int floatCount = acquData.Length / 4;
            float[] floatData = new float[floatCount];
            for (int i = 0; i < floatCount; i++)
            {
                floatData[i] = BitConverter.ToSingle(acquData, i * 4);
            }

            XT_109_SL_3_ReceiveData entity = new XT_109_SL_3_ReceiveData()
            {
                Id = _equipId,
                CreationTime = DateTime.Now,
                SimuTestSysld = simuTestSysId,
                DevTypeld = devTypeId,
                Compld = compNumber,
                LocalOrRemote = workStyle[0],
                WorkPattern = workStyle[1],
                Reserved = workStyle[2],
                PhysicalQuantityCount = ulPhysicalQuantityCount
            };

            // 使用反射将后面指定数量个物理量数据进行填充
            PropertyInfo[] properties = typeof(XT_109_SL_3_ReceiveData).GetProperties();

            // 使用循环将数组值赋给类属性
            for (int i = 0; i < floatData.Length; i++)
            {
                // 跳过前面9个属性,索引10开始设置
                properties[i + 9].SetValue(entity, floatData[i]);
            }

            // 将试验数据记录数据库
            XT_109_SL_3_ReceiveData receive = await SqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();

            // 将试验数据的数据部分推送到mqtt给前端进行展示
            await TestDataPublishToMQTT(receive);

            #endregion

            #region 处理不健康状态

            // 获取自检状态异常
            string exception = GetDevHealthExceptionName(stateType);

            if (string.IsNullOrEmpty(exception))
            {
                // 将异常记录到redis
                EquipNotice equipNotice = await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, simuTestSysId, devTypeId, compId, _equipId, exception);
                // 将异常发布到mqtt
                await ReceiveHelper.ExceptionPublishToMQTT(_mqttExplorer, equipNotice, _equipId);
                // 将异常记录到数据库
                await ReceiveHelper.RecordExceptionToDB(SqlSugarClient, equipNotice);
            }

            #endregion

            return _equipId;
        }

        private async Task TestDataPublishToMQTT(XT_109_SL_3_ReceiveData receive)
        {
            XT_109_SL_3_TestAnalyseJob job = new XT_109_SL_3_TestAnalyseJob();
            ApiResponse ar = job.GetResponseForPushData(receive, null!);
            string topic = IotTopicBuilder
            .CreateIotBuilder()
            .WithPrefix(TopicType.Iot)
            .WithDirection(MqttDirection.Up)
            .WithTag(MqttTag.Show)
            .WithDeviceType(EquipConnType.IotServer.ToString())
            .WithUri(_equipId.ToString())
            .Build();
            // 推送ar的Data用于展示
            await _mqttExplorer.PublishAsync(topic, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ar.data)));
        }
    }
}
