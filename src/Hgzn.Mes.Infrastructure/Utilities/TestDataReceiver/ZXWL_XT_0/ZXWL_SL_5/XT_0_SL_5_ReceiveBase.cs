using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_0.ZXWL_SL_5
{
    public class XT_0_SL_5_ReceiveBase : ReceiveBase
    {
        public XT_0_SL_5_ReceiveBase(Guid equipId, ISqlSugarClient client, IConnectionMultiplexer connectionMultiplexer, IMqttExplorer mqttExplorer) : base(equipId, client, connectionMultiplexer, mqttExplorer) { }

        /// <summary>
        /// 移动电源_ZXWL-SL-5
        /// </summary>
        /// <param name="ulDevHealthState"></param>
        /// <returns></returns>
        protected List<string> GetSL_5HealthExceptionName(byte[] ulDevHealthState)
        {
            List<string> exceptions = new List<string>();
            if (ulDevHealthState[0] == 0)
            {
                exceptions.Add(General_StatusType.DeviceHealthNotAcquired.GetDescription());
                return exceptions;
            }
            if (ulDevHealthState[1] == (byte)General_Status.Abnormal) exceptions.Add("工作状态" + General_Status.Abnormal.GetDescription());
            return exceptions;
        }
    }
}
