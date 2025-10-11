using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver
{
    public class OnlineReceiveDispatch : BaseReceive
    {
        public OnlineReceiveDispatch(Guid equipId,
            ISqlSugarClient _client,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer) : base(equipId, _client, connectionMultiplexer, mqttExplorer) { }

        public async Task Handle(byte[] msg)
        {
            byte[] newBuffer;
            DateTime time;
            uint bufferLength;
            if (ReceiveHelper.GetMessage(msg, out bufferLength, out time, out newBuffer))
            {
                LoggerAdapter.LogInformation($"AG - WebApi收到转发数据内容: {BitConverter.ToString(msg, 0, (int)bufferLength).Replace("-", " ")}");

                // 使用 eventData.Data 作为 buffer
                byte[] buffer = newBuffer;

                // 仿真试验系统识别编码
                byte simuTestSysId = buffer[0];

                // 设备类型识别编码
                byte devTypeId = buffer[1];

                // 如果我收到了某个系统的某个类型,那么我标记它在30秒内在线,我会在Redis中创建一个寿命为30秒的心跳
                await ReceiveHelper.LiveRecordToRedis(_connectionMultiplexer, simuTestSysId, devTypeId, _equipId, time);

                //增加试验设备记录，后期根据这个获取对应系统的对应设备的数据
                byte[] systemAndDeviceType = new byte[22];
                Buffer.BlockCopy(buffer, 0, systemAndDeviceType, 0, 22);
                if (!ReceiveHelper.ReceiveTestSystem.Contains(systemAndDeviceType))
                {
                    ReceiveHelper.ReceiveTestSystem.Enqueue(systemAndDeviceType);
                    byte[] compId = new byte[20];
                    Buffer.BlockCopy(buffer, 2, compId, 0, 20);
                    string compNumber = Encoding.ASCII.GetString(compId).Trim('\0');
                    await _sqlSugarClient.Insertable(new TestEquipData()
                    {
                        TestEquip = systemAndDeviceType,
                        SimuTestSysld = simuTestSysId,
                        DevTypeld = devTypeId,
                        Compld = compNumber,
                    }).ExecuteCommandAsync();
                }
                
                // 根据仿真试验系统与设备类型,通过工厂创建各自的解析类
                // 以上所有系统固定占2个字节
                OnlineReceiveFactory onlineReceiveFactory = new OnlineReceiveFactory(_equipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer);
                IOnlineReceive onlineReceive = onlineReceiveFactory.GetOrCreateOnlineReceive(simuTestSysId, devTypeId);
                await onlineReceive.Handle(newBuffer, time);
            }

        }
    }
}
