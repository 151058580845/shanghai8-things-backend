using Hgzn.Mes.Domain.Events;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared;
using MediatR;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Newtonsoft.Json;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Domain.ValueObjects.Message;
using System.Reflection;
using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_307_ReceiveDatas;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipData.ReceiveData.XT_109_ReceiveDatas;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_307.ZXWL_SL_1
{
    public class XT_307_SL_1_OnlineReceive : XT_307_SL_1_ReceiveBase, IOnlineReceive
    {
        public XT_307_SL_1_OnlineReceive(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer) : base(equipId, _client, connectionMultiplexer, mqttExplorer) { }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="eventData"></param>
        /// simuTestSysId   字节1     固定
        /// devTypeId       字节1     固定
        /// compId          字节20    固定
        /// workStyle       字节10    不固定
        /// devHealthState   字节8     不固定
        /// acquData        字节2800  不固定
        public async Task<Guid> Handle(byte[] msg, DateTime sendTime)
        {
            string data = Encoding.UTF8.GetString(msg);

            // 使用 eventData.Data 作为 buffer
            byte[] buffer = msg;

            // 仿真试验系统识别编码
            byte simuTestSysId = buffer[0];
            LoggerAdapter.LogDebug($"AG - 远程解析 - 仿真试验系统识别编码:{simuTestSysId}");

            // 设备类型识别编码
            byte devTypeId = buffer[1];
            LoggerAdapter.LogDebug($"AG - 远程解析 - 设备类型识别编码:{devTypeId}");

            // 本机识别编码
            byte[] compId = new byte[20];
            Buffer.BlockCopy(buffer, 2, compId, 0, 20);
            string compNumber = Encoding.ASCII.GetString(compId).Trim('\0');
            // 最新需求,记录数据库的时候去掉引号保存
            compNumber = compNumber.Trim('"');
            LoggerAdapter.LogDebug($"AG - 远程解析 - 本机识别编码:{compNumber}");

            // 工作模式信息
            byte[] workStyle = new byte[10];
            Buffer.BlockCopy(buffer, 22, workStyle, 0, 10);
            LoggerAdapter.LogDebug($"AG - 远程解析 - 工作模式信息:{string.Join(", ", workStyle.Select(b => (int)b))}");

            // *** 健康状态信息
            // 状态类型
            byte stateType = buffer[32];
            LoggerAdapter.LogDebug($"AG - 远程解析 - 健康状态信息:{stateType}");

            // 自检状态1个uint,表中是4位的ulong,在C#中,直接用uint代替

            byte[] devHealthState = new byte[4];
            Buffer.BlockCopy(buffer, 33, devHealthState, 0, 4);
            uint ulDevHealthState = BitConverter.ToUInt32(devHealthState, 0);

            // 电源电压状态1个uint,表中是4位的ulong,在C#中,直接用uint代替
            byte[] supplyVoltageState = new byte[4];
            Buffer.BlockCopy(buffer, 37, devHealthState, 0, 4);
            uint ulSupplyVoltageState = BitConverter.ToUInt32(supplyVoltageState, 0);


            // *** 物理量数量
            byte[] physicalQuantityCount = new byte[4];
            Buffer.BlockCopy(buffer, 41, physicalQuantityCount, 0, 4);
            uint ulPhysicalQuantityCount = BitConverter.ToUInt32(physicalQuantityCount, 0);
            ulPhysicalQuantityCount -= 1; // 减去1是因为最后一个物理量是运行时间,这个是后加的,且属性类型是uint,不好转成float一起赋值,在赋值完普通物理量后我会单独赋值
            LoggerAdapter.LogDebug($"AG - 远程解析 - 物理量数量(不包含运行时长):{ulPhysicalQuantityCount}");

            // 剩余的都给物理量
            byte[] acquData = new byte[ulPhysicalQuantityCount * 4];
            Buffer.BlockCopy(buffer, 45, acquData, 0, acquData.Length);

            // *** 运行时间
            // 计算需要读取的起始位置和结束位置
            int startPosition = (int)(ulPhysicalQuantityCount * 4 + 45);
            int requiredLength = startPosition + 4; // runTime 需要 4 字节
            uint ulRunTime = 0;
            if (buffer.Length >= requiredLength)
            {
                byte[] runTime = new byte[4];
                Buffer.BlockCopy(buffer, startPosition, runTime, 0, 4);
                ulRunTime = BitConverter.ToUInt32(runTime, 0);
            }
            LoggerAdapter.LogDebug($"AG - 远程解析 - 运行时间:{ulRunTime}");

            // 将 byte[] 转换为 float[] , 每个 float 占用 4 字节
            int floatCount = acquData.Length / 4;
            float[] floatData = new float[floatCount];
            for (int i = 0; i < floatCount; i++)
            {
                floatData[i] = BitConverter.ToSingle(acquData, i * 4);
            }

            XT_307_SL_1_ReceiveData entity = new XT_307_SL_1_ReceiveData()
            {
                Id = _equipId,
                CreationTime = sendTime.ToString("yyyy-MM-dd hh:mm:ss"),
                SimuTestSysld = simuTestSysId,
                DevTypeld = devTypeId,
                Compld = compNumber,
                MicroWare = workStyle[0],
                Channel = workStyle[1],
                ModelValid = workStyle[2],
                ArrayEndPolarizationMode = workStyle[3],
                ArrayEndPowerMode = workStyle[4],
                ArrayChannelMultiplexing = workStyle[5],
                ChannelPolarizationMode1 = workStyle[6],
                ChannelPolarizationMode2 = workStyle[7],
                ChannelPowerMode = workStyle[8],
                Reserved = workStyle[9],
                StateType = stateType,
                SelfTest = ulDevHealthState,
                SupplyVoltageState = ulSupplyVoltageState,
                PhysicalQuantityCount = ulPhysicalQuantityCount,
                RunTime = ulRunTime,
            };

            // 使用反射将后面指定数量个物理量数据进行填充
            PropertyInfo[] properties = typeof(XT_307_SL_1_ReceiveData).GetProperties();

            // 使用循环将数组值赋给类属性
            for (int i = 0; i < floatData.Length; i++)
            {
                // 跳过前面19个属性,索引20开始设置---------------------------------------------这里可能有问题但是314已经对过了,我暂时不改,应该是+20才对吧
                properties[i + 19].SetValue(entity, floatData[i]);
            }

            // 这里根据设备的不同有些设备只有电源电压参数,所以去获取电源电压异常,有些设备只有自检状态参数,所以去获取自检状态异常
            // 这里定义如果该设备的值是0,则检查自检状态异常,如果是1则检查电源电压异常
            List<string> exception = new List<string>();
            if (DeviceStateTag.ContainsKey(compNumber))
            {
                if (DeviceStateTag[compNumber] == 0)
                    // 获取自检状态异常
                    exception = GetDevHealthExceptionName(ulDevHealthState);
                else if (DeviceStateTag[compNumber] == 1)
                    // 获取电源电压异常
                    exception = GetSupplyVoltageExceptionName(ulSupplyVoltageState);
            }
            LoggerAdapter.LogDebug($"AG - 远程解析 - 健康检查异常列表（共 {exception.Count} 条）:\n{string.Join("\n", exception)}");

            // 将试验数据记录数据库
            XT_307_SL_1_ReceiveData receive = await _sqlSugarClient.Insertable(entity).ExecuteReturnEntityAsync();
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
            LoggerAdapter.LogDebug($"AG - 远程解析 - 写入数据库完成");
            // 将试验数据的数据部分推送到mqtt给前端进行展示
            // await TestDataPublishToMQTT(receive);
            // 将异常和运行时长记录到redis
            await ReceiveHelper.ExceptionRecordToRedis(_connectionMultiplexer, simuTestSysId, devTypeId, compId, _equipId, exception, sendTime, ulRunTime);

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

            LoggerAdapter.LogDebug($"AG - 远程解析 - 完毕");
            return _equipId;
        }

        private async Task TestDataPublishToMQTT(XT_307_SL_1_ReceiveData receive)
        {
            XT_307_SL_1_TestAnalyseJob job = new XT_307_SL_1_TestAnalyseJob();
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