using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_109_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_202_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_202.ZXWL_SL_3
{
    public class XT_202_SL_3_OnlineReceive : BaseReceive, IOnlineReceive
    {
        private Func<short[], List<string>> _getHealthException;
        private const int _WORKSTYLEANALYSISLENGTH = 10;
        private const int _STATETYPEANALYSISLENGTH = 11;
        private int _workStyleLength;
        private int _stateTypeLength;
        public XT_202_SL_3_OnlineReceive(
            Guid equipId,
            ISqlSugarClient client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer,
            Func<short[], List<string>> getHealthException, int workStyleLength, int stateTypeLength) : base(equipId, client, connectionMultiplexer, mqttExplorer)
        {
            this._getHealthException = getHealthException;
            this._workStyleLength = workStyleLength;
            this._stateTypeLength = stateTypeLength;
        }

        public async Task<Guid> Handle(byte[] msg, DateTime sendTime)
        {
            string data = Encoding.UTF8.GetString(msg);

            // 使用 eventData.Data 作为 buffer
            byte[] buffer = msg;
            // 仿真试验系统识别编码
            byte simuTestSysId = buffer[0];
            LoggerAdapter.LogInformation($"AG - 远程解析 - 仿真试验系统识别编码:{simuTestSysId}");
            // 设备类型识别编码
            byte devTypeId = buffer[1];
            LoggerAdapter.LogInformation($"AG - 远程解析 - 设备类型识别编码:{devTypeId}");
            // 本机识别编码
            byte[] compId = new byte[20];
            Buffer.BlockCopy(buffer, 2, compId, 0, 20);
            string compNumber = Encoding.ASCII.GetString(compId).Trim('\0');
            // 最新需求,记录数据库的时候去掉引号保存
            compNumber = compNumber.Trim('"');
            LoggerAdapter.LogInformation($"AG - 远程解析 - 本机识别编码:{compNumber}");

            // 工作模式信息
            byte[] workStyle = new byte[_WORKSTYLEANALYSISLENGTH];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, _WORKSTYLEANALYSISLENGTH);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 工作模式信息:{string.Join(", ", workStyle.Select(x => (int)x))}");

            // *** 健康状态信息
            // 状态类型
            short[] healthStatus = new short[5];
            Buffer.BlockCopy(buffer, 21 + _WORKSTYLEANALYSISLENGTH + 1, healthStatus, 0, 10);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 健康状态信息:{string.Join(", ", healthStatus.Select(x => (int)x))}");

            // *** 物理量
            // 剩余的都给物理量
            uint ulPhysicalQuantityCount;
            float[] floatData = GetPhysicalQuantity(buffer, 21 + _WORKSTYLEANALYSISLENGTH + _STATETYPEANALYSISLENGTH, out ulPhysicalQuantityCount);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 物理量数量(不包含运行时长):{ulPhysicalQuantityCount}");

            // *** 运行时间
            // 计算需要拷贝的起始位置和需要的总长度
            int startPosition = (int)((ulPhysicalQuantityCount + 1) * 4 + 21 + _WORKSTYLEANALYSISLENGTH + _STATETYPEANALYSISLENGTH);
            int requiredLength = startPosition + 4;
            uint ulRunTime = 0;
            if (buffer.Length >= requiredLength)
            {
                byte[] runTime = new byte[4];
                Buffer.BlockCopy(buffer, startPosition, runTime, 0, 4);
                ulRunTime = BitConverter.ToUInt32(runTime, 0);
            }
            LoggerAdapter.LogInformation($"AG - 远程解析 - 运行时长:{ulRunTime}");

            // *** 构建entity
            XT_202_SL_3_ReceiveData entity = new XT_202_SL_3_ReceiveData();
            // 使用反射将后面指定数量个物理量数据进行填充
            PropertyInfo[] properties = typeof(XT_202_SL_3_ReceiveData).GetProperties();
            // properties[0] 是 Id 字段，会自动生成唯一的GUID，不应该赋值为设备ID
            properties[1].SetValue(entity, sendTime);
            properties[2].SetValue(entity, simuTestSysId);
            properties[3].SetValue(entity, devTypeId);
            properties[4].SetValue(entity, compNumber);

            // 填充工作模式信息,虽然我获取10个,但是实际是几个我就设置几个
            for (int i = 0; i < _workStyleLength; i++)
            {
                properties[5 + i].SetValue(entity, workStyle[i]);
            }

            // 填充健康状态信息
            for (int i = 0; i < healthStatus.Length; i++)
            {
                properties[5 + _workStyleLength + i].SetValue(entity, healthStatus[i]);
            }

            // 填充物理量数量
            properties[4 + _workStyleLength + _stateTypeLength].SetValue(entity, ulPhysicalQuantityCount);

            // 填充物理量
            // 使用循环将数组值赋给类属性
            for (int i = 0; i < floatData.Length; i++)
            {
                // 跳过前面的属性
                properties[5 + _workStyleLength + _stateTypeLength + i].SetValue(entity, floatData[i]);
            }

            // 填充运行时间
            // 验证运行时间：如果超过 24 小时（86400 秒），则认为数据错误，填充 0
            const uint MAX_RUNTIME_SECONDS = 24 * 60 * 60; // 24小时 = 86400秒
            uint validatedRunTime = ulRunTime;
            
            if (ulRunTime > MAX_RUNTIME_SECONDS)
            {
                LoggerAdapter.LogWarning($"AG - 远程解析 - 运行时间异常：{ulRunTime}秒（超过24小时），已重置为0");
                validatedRunTime = 0;
            }
            
            // 使用属性名获取 RunTime 属性，避免因物理量数量变化导致索引错误
            PropertyInfo? runTimeProperty = typeof(XT_202_SL_3_ReceiveData).GetProperty("RunTime");
            if (runTimeProperty != null)
            {
                runTimeProperty.SetValue(entity, validatedRunTime);
            }
            else
            {
                LoggerAdapter.LogWarning($"AG - 远程解析 - 未找到 RunTime 属性，使用索引方式赋值");
                properties[5 + _workStyleLength + _stateTypeLength + floatData.Length].SetValue(entity, validatedRunTime);
            }

            // *** 处理异常信息
            List<string> exception = _getHealthException(healthStatus);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 健康检查异常列表（共 {exception.Count} 条）:\n{string.Join("\n", exception)}");

            // 将试验数据记录数据库
            XT_202_SL_3_ReceiveData receive = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
            // 分表不在远程解析时记录
            //Receive receive = new Receive()
            //{
            //    SimuTestSysld = simuTestSysId,
            //    DevTypeld = devTypeId,
            //    Compld = compNumber,
            //    CreateTime = sendTime,
            //    Content = entity,
            //};
            //_sqlSugarClient.Insertable(new List<Receive>() { receive }).SplitTable().ExecuteCommand();
            LoggerAdapter.LogInformation($"AG - 远程解析 - 写入数据库完成");
            // 将试验数据的数据部分推送到mqtt给前端进行展示(暂时不进行数据展示)
            // await TestDataPublishToMQTT(receive);
            // 将异常和运行时长记录到redis
            await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, _sqlSugarClient, simuTestSysId, devTypeId, compId, _equipId, exception, sendTime, validatedRunTime);

            if (exception.Count > 0)
            {
                // 新建通知
                EquipNotice equipNotice = new EquipNotice()
                {
                    EquipId = _equipId,
                    SendTime = sendTime,
                    NoticeType = EquipNoticeType.Alarm,
                    Title = "Receive Alarm",
                    Content = JsonConvert.SerializeObject(exception),
                    Description = "",
                    SimuTestSysId = simuTestSysId,
                };
                // 将异常发布到mqtt,发布后会由webapi将异常记录到数据库
                await ReceiveHelper.ExceptionPublishToMQTT(_mqttExplorer, equipNotice, _equipId);
            }
            LoggerAdapter.LogInformation($"AG - 远程解析 - 完毕");
            return _equipId;
        }
    }
}
