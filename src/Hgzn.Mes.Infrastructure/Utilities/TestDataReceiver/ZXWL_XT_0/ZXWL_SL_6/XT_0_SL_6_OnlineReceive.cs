using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_0_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using Newtonsoft.Json;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_0.ZXWL_SL_6
{
    public class XT_0_SL_6_OnlineReceive : XT_0_SL_6_ReceiveBase, IOnlineReceive
    {
        public XT_0_SL_6_OnlineReceive(Guid equipId, ISqlSugarClient client, IConnectionMultiplexer connectionMultiplexer, IMqttExplorer mqttExplorer) : base(equipId, client, connectionMultiplexer, mqttExplorer) { }

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
            byte[] workStyle = new byte[10];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, 10);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 工作模式信息:{string.Join(", ", workStyle.Select(b => (int)b))}");

            // *** 健康状态信息
            // 状态类型
            byte stateType = buffer[32];
            ushort selfTestStatus = BitConverter.ToUInt16(buffer, 33);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 健康状态信息:{(int)stateType}");

            // *** 物理量
            // 剩余的都给物理量
            byte[] physicalQuantityCount = new byte[4];
            Buffer.BlockCopy(buffer, 41, physicalQuantityCount, 0, 4);
            uint ulPhysicalQuantityCount = BitConverter.ToUInt32(physicalQuantityCount, 0);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 物理量数量(不包含运行时长):{ulPhysicalQuantityCount}");

            // *** 运行时间
            byte[] runTime = new byte[4];
            Buffer.BlockCopy(buffer, 105, runTime, 0, 4);
            uint ulRunTime = BitConverter.ToUInt32(runTime, 0);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 运行时间:{ulRunTime}");

            // 验证运行时间：如果超过 24 小时（86400 秒），则认为数据错误，填充 0
            const uint MAX_RUNTIME_SECONDS = 24 * 60 * 60; // 24小时 = 86400秒
            uint validatedRunTime = ulRunTime;
            
            if (ulRunTime > MAX_RUNTIME_SECONDS)
            {
                LoggerAdapter.LogWarning($"AG - 远程解析 - 运行时间异常：{ulRunTime}秒（超过24小时），已重置为0");
                validatedRunTime = 0;
            }

            // *** 构建entity
            XT_0_SL_6_ReceiveData entity = new XT_0_SL_6_ReceiveData()
            {
                // Id 由 UniversalEntity 自动生成唯一的 GUID，不应该手动设置为 _equipId
                CreationTime = sendTime,
                SimuTestSysld = simuTestSysId,
                DevTypeld = devTypeId,
                Compld = compNumber,
                LocalOrRemote = workStyle[0],
                ChannelEnabled = workStyle[1],
                OperationMode = workStyle[2],
                FrequencyMeasurementEnabled = workStyle[3],
                DopplerDataValid = workStyle[4],
                Reserved = workStyle[5],
                StatusType = stateType,
                SelfTestStatus = selfTestStatus,
                PhysicalParameterCount = ulPhysicalQuantityCount,
                // 这里一下byte一下float的没啥规律,且只有这一组是这样的,干脆全部手写
                MainCtrlDrfm1NetStatus = buffer[45],
                MainCtrlDrfm2NetStatus = buffer[46],
                Fpga1Status = buffer[47],
                Fpga2Status = buffer[48],
                Dsp1Status = buffer[49],
                Dsp2Status = buffer[50],
                Fpga1Adc1Dac1Status = buffer[51],
                Fpga2Adc2Dac2Status = buffer[52],
                Fpga1Dsp1SrioStatus = buffer[53],
                Fpga2Dsp2SrioStatus = buffer[54],
                Drfm1Temperature = BitConverter.ToSingle(buffer, 55),
                Drfm2Temperature = BitConverter.ToSingle(buffer, 59),
                MicrowaveCtrlStatus = buffer[63],
                FrequencySynthesizerStatus = buffer[64],
                IfInputPower = BitConverter.ToSingle(buffer, 65),
                IfOutputPower = BitConverter.ToSingle(buffer, 69),
                Backup1 = BitConverter.ToSingle(buffer, 73),
                Backup2 = BitConverter.ToSingle(buffer, 77),
                Backup3 = BitConverter.ToSingle(buffer, 83),
                Backup4 = BitConverter.ToSingle(buffer, 85),
                Backup5 = BitConverter.ToSingle(buffer, 89),
                Backup6 = BitConverter.ToSingle(buffer, 93),
                Backup7 = BitConverter.ToSingle(buffer, 97),
                Backup8 = BitConverter.ToSingle(buffer, 101),
                RunTime = validatedRunTime,
            };

            // *** 处理异常信息
            byte[] healthInfo = new byte[3];
            Buffer.BlockCopy(buffer, 32, healthInfo, 0, 3);
            List<string> exception = GetXT_0_SL_6HealthExceptionName(healthInfo);
            LoggerAdapter.LogInformation($"AG - 远程解析 - 健康检查异常列表（共 {exception.Count} 条）:\n{string.Join("\n", exception)}");
            // 将试验数据记录数据库
            Receive receive = new Receive()
            {
                SimuTestSysld = simuTestSysId,
                DevTypeld = devTypeId,
                Compld = compNumber,
                CreateTime = sendTime,
                Content = entity,
            };
            _sqlSugarClient.Insertable(new List<Receive>() { receive }).SplitTable().ExecuteCommand();
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
