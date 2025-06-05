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

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_0.ZXWL_SL_6
{
    public class XT_0_SL_6_OnlineReceive : XT_0_SL_6_ReceiveBase, IOnlineReceive
    {
        public XT_0_SL_6_OnlineReceive(Guid equipId, ISqlSugarClient client, IConnectionMultiplexer connectionMultiplexer, IMqttExplorer mqttExplorer) : base(equipId, client, connectionMultiplexer, mqttExplorer) { }

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
            byte[] workStyle = new byte[10];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, 10);


            // *** 健康状态信息
            // 状态类型
            byte stateType = buffer[32];
            ushort selfTestStatus = BitConverter.ToUInt16(buffer, 33);


            // *** 物理量
            // 剩余的都给物理量
            byte[] physicalQuantityCount = new byte[4];
            Buffer.BlockCopy(buffer, 35, physicalQuantityCount, 0, 4);
            uint ulPhysicalQuantityCount = BitConverter.ToUInt32(physicalQuantityCount, 0);

            // *** 构建entity
            XT_0_SL_6_ReceiveData entity = new XT_0_SL_6_ReceiveData()
            {
                Id = _equipId,
                CreationTime = DateTime.Now,
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
                MainCtrlDrfm1NetStatus = buffer[39],
                MainCtrlDrfm2NetStatus = buffer[40],
                Fpga1Status = buffer[41],
                Fpga2Status = buffer[42],
                Dsp1Status = buffer[43],
                Dsp2Status = buffer[44],
                Fpga1Adc1Dac1Status = buffer[45],
                Fpga2Adc2Dac2Status = buffer[46],
                Fpga1Dsp1SrioStatus = buffer[47],
                Fpga2Dsp2SrioStatus = buffer[48],
                Drfm1Temperature = BitConverter.ToSingle(buffer, 49),
                Drfm2Temperature = BitConverter.ToSingle(buffer, 53),
                MicrowaveCtrlStatus = buffer[57],
                FrequencySynthesizerStatus = buffer[58],
                IfInputPower = BitConverter.ToSingle(buffer, 59),
                IfOutputPower = BitConverter.ToSingle(buffer, 63),
                Backup1 = BitConverter.ToSingle(buffer, 67),
                Backup2 = BitConverter.ToSingle(buffer, 71),
                Backup3 = BitConverter.ToSingle(buffer, 75),
                Backup4 = BitConverter.ToSingle(buffer, 79),
                Backup5 = BitConverter.ToSingle(buffer, 83),
                Backup6 = BitConverter.ToSingle(buffer, 87),
                Backup7 = BitConverter.ToSingle(buffer, 91),
                Backup8 = BitConverter.ToSingle(buffer, 95),
            };

            // *** 处理异常信息
            byte[] healthInfo = new byte[3];
            Buffer.BlockCopy(buffer, 32, workStyle, 0, 3);
            List<string> exception = GetXT_0_SL_6HealthExceptionName(healthInfo);
            // 将试验数据记录数据库
            XT_0_SL_6_ReceiveData receive = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
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
