using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Newtonsoft.Json;
using SqlSugar;
using StackExchange.Redis;
using System.Reflection;
using System.Text;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common
{
    public class GeneralOnlineReceive<T> : BaseReceive, IOnlineReceive where T : class, new()
    {
        private Func<byte[], List<string>> _getHealthException;
        private const int _WORKSTYLEANALYSISLENGTH = 10;
        private const int _STATETYPEANALYSISLENGTH = 9;
        private int _workStyleLength;
        private int _stateTypeLength;

        public GeneralOnlineReceive(
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
            LoggerAdapter.LogInformation($"AG - 远程解析 - 工作模式信息:{string.Join(", ", workStyle.Select(b => (int)b))}");

            // *** 健康状态信息
            // 状态类型
            byte[] stateType = new byte[_STATETYPEANALYSISLENGTH];
            Buffer.BlockCopy(buffer, 22 + _WORKSTYLEANALYSISLENGTH, stateType, 0, _STATETYPEANALYSISLENGTH);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 健康状态信息:{string.Join(", ", stateType.Select(b => (int)b))}");

            // *** 物理量
            // 剩余的都给物理量
            uint ulPhysicalQuantityCount;
            float[] floatData = GetPhysicalQuantity(buffer, 22 + _WORKSTYLEANALYSISLENGTH + _STATETYPEANALYSISLENGTH, out ulPhysicalQuantityCount);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 物理量数量(不包含运行时长):{ulPhysicalQuantityCount}");

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
            LoggerAdapter.LogInformation($"AG - 远程解析 - 运行时间:{ulRunTime}");

            // *** 构建entity
            T entity = new T();
            // 使用反射将后面指定数量个物理量数据进行填充
            PropertyInfo[] properties = typeof(T).GetProperties();
            properties[0].SetValue(entity, _equipId);
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
            for (int i = 0; i < _stateTypeLength; i++)
            {
                properties[5 + _workStyleLength + i].SetValue(entity, stateType[i]);
            }

            // 填充物理量数量
            properties[5 + _workStyleLength + _stateTypeLength].SetValue(entity, ulPhysicalQuantityCount);

            // 填充物理量
            // 使用循环将数组值赋给类属性
            for (int i = 0; i < floatData.Length; i++)
            {
                // 跳过前面的属性
                properties[6 + _workStyleLength + _stateTypeLength + i].SetValue(entity, floatData[i]);
            }

            // 填充运行时间
            properties[6 + _workStyleLength + _stateTypeLength + floatData.Length].SetValue(entity, ulRunTime);

            // *** 处理异常信息
            List<string> exception = _getHealthException(stateType);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 健康检查异常列表（共 {exception.Count} 条）:\n{string.Join("\n", exception)}");

            // 将试验数据记录数据库
            T receive = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
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
            await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, _sqlSugarClient, simuTestSysId, devTypeId, compId, _equipId, exception, sendTime, ulRunTime);

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
