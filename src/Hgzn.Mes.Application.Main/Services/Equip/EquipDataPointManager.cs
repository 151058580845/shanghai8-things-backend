using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.ProtocolManagers;
using Hgzn.Mes.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Services.Equip
{

    public class EquipDataPointManager
    {
        /// <summary>
        /// 开始采集
        /// </summary>
        /// <param name="dataPoint">采集点位</param>
        /// <returns></returns>
        public async Task StartCollectAsync(EquipDataPoint dataPoint)
        {
            if (await EquipControlHelp.IsConnectedAsync(dataPoint.ConnectionId))
            {
                await EquipControlHelp.AddDevicePointAsync(dataPoint.ConnectionId, dataPoint);
            }
            else
            {
                throw new Exception("找不到该连接");
            }
        }


        /// <summary>
        /// 停止采集
        /// </summary>
        /// <param name="dataPoint"></param>
        public async Task StopCollectAsync(EquipDataPoint dataPoint)
        {
            if (await EquipControlHelp.IsConnectedAsync(dataPoint.ConnectionId))
            {
                await EquipControlHelp.RemoveDevicePointAsync(dataPoint.ConnectionId, dataPoint);
            }
            else
            {
                throw new ApplicationException("找不到该连接");
            }
        }
    }
}
