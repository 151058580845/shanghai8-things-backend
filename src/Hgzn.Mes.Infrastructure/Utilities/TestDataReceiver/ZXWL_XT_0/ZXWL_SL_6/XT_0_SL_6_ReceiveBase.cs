using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_0.ZXWL_SL_6
{
    public class XT_0_SL_6_ReceiveBase : BaseReceive
    {
        public XT_0_SL_6_ReceiveBase(Guid equipId, ISqlSugarClient client, IConnectionMultiplexer connectionMultiplexer, IMqttExplorer mqttExplorer) : base(equipId, client, connectionMultiplexer, mqttExplorer) { }

        /// <summary>
        /// 雷达源_ZXWL-SL-6
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        public List<string> GetXT_0_SL_6HealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0)
            {
                exceptions.Add(General_StatusType.DeviceHealthNotAcquired.GetDescription());
                return exceptions;
            }
            // 虽然协议上,这里是ushort类型,但是只有第一个字节有数据,所以我可以用byte来代替进行接收
            XT_0_SL_6DeviceStatus status = (XT_0_SL_6DeviceStatus)ulDevHealthState[1];
            foreach (XT_0_SL_6DeviceStatus flag in Enum.GetValues(typeof(XT_0_SL_6DeviceStatus)))
            {
                if (status.HasFlag(flag))
                    exceptions.Add(flag.GetDescription());
            }
            // 第二个字节是备用的,不进行解析
            return exceptions;
        }
    }
}
