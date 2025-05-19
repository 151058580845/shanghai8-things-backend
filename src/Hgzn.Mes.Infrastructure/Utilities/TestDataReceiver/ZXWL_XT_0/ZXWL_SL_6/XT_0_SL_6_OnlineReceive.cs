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
            byte[] workStyle = new byte[6];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, 6);


            // *** 健康状态信息
            // 状态类型
            byte stateType = buffer[28];
            ushort selfTestStatus = BitConverter.ToUInt16(buffer, 29);


            // *** 物理量
            // 剩余的都给物理量
            byte[] physicalQuantityCount = new byte[4];
            Buffer.BlockCopy(buffer, 31, physicalQuantityCount, 0, 4);
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
                MainCtrlDrfm1NetStatus = buffer[35],
                MainCtrlDrfm2NetStatus = buffer[36],
                Fpga1Status = buffer[37],
                Fpga2Status = buffer[38],
                Dsp1Status = buffer[39],
                Dsp2Status = buffer[40],
                Fpga1Adc1Dac1Status = buffer[41],
                Fpga2Adc2Dac2Status = buffer[42],
                Fpga1Dsp1SrioStatus = buffer[43],
                Fpga2Dsp2SrioStatus = buffer[44],
                Drfm1Temperature = BitConverter.ToSingle(buffer, 45),
                Drfm2Temperature = BitConverter.ToSingle(buffer, 49),
                MicrowaveCtrlStatus = buffer[53],
                FrequencySynthesizerStatus = buffer[54],
                IfInputPower = BitConverter.ToSingle(buffer, 55),
                IfOutputPower = BitConverter.ToSingle(buffer, 59),
                Backup1 = BitConverter.ToSingle(buffer, 63),
                Backup2 = BitConverter.ToSingle(buffer, 67),
                Backup3 = BitConverter.ToSingle(buffer, 71),
                Backup4 = BitConverter.ToSingle(buffer, 75),
                Backup5 = BitConverter.ToSingle(buffer, 79),
                Backup6 = BitConverter.ToSingle(buffer, 83),
                Backup7 = BitConverter.ToSingle(buffer, 87),
                Backup8 = BitConverter.ToSingle(buffer, 91),
            };

            // *** 处理异常信息
            byte[] healthInfo = new byte[3];
            Buffer.BlockCopy(buffer, 28, workStyle, 0, 3);
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
