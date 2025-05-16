using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using StackExchange.Redis;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_109.ZXWL_SL_3
{
    public class XT_109_SL_3_LocalReceive : XT_109_SL_3_ReceiveBase, ILocalReceive
    {
        public XT_109_SL_3_LocalReceive(Guid equipId,
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
            byte[] stateType = new byte[2];
            Buffer.BlockCopy(buffer, 25, workStyle, 0, 2);

            // 健康状态信息第0为为1表示获取到了健康状态
            if (stateType[0] == 1)
            {
                string exception = GetDevHealthExceptionName(stateType[1]);
                if (!string.IsNullOrEmpty(exception) && stateType[1] != 0)
                {
                    EquipNotice equipNotice = new EquipNotice()
                    {
                        EquipId = _equipId,
                        SendTime = DateTime.Now,
                        NoticeType = EquipNoticeType.Alarm,
                        Title = "Receive Alarm",
                        Content = exception,
                        Description = "",
                    };

                    // 将异常记录到数据库
                    equipNotice.Id = Guid.NewGuid();
                    EquipNotice sequipNotice = await _sqlSugarClient.Insertable(equipNotice).ExecuteReturnEntityAsync();
                }
            }

            return _equipId;
        }
    }
}
