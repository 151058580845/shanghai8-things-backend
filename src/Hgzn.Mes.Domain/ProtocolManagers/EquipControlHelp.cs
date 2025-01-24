using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.ProtocolManagers
{
    public class EquipControlHelp
    {
        private static readonly ConcurrentDictionary<Guid, IEquipManager> EquipManagers = new();
        private static readonly ConcurrentDictionary<Guid, DataPointStatus> DataPointStatusDict = new();

        /// <summary>
        /// 获取当前点位的采集状态
        /// </summary>
        /// <param name="dataPointId"></param>
        /// <returns></returns>
        public static DataPointStatus GetStatusByPointId(Guid dataPointId)
        {
            return DataPointStatusDict.GetValueOrDefault(dataPointId, DataPointStatus.None);
        }

        /// <summary>
        /// 判断是否是连接状态
        /// </summary>
        /// <param name="connectId"></param>
        /// <returns></returns>
        public static async Task<bool> IsConnectedAsync(Guid connectId)
        {
            if (EquipManagers.TryGetValue(connectId, out IEquipManager? deviceManager))
            {
                return await deviceManager.IsConnectedAsync();
            }

            return false;
        }

        /// <summary>
        /// 给采集连接添加一个采集点位
        /// </summary>
        /// <param name="connectId"></param>
        /// <param name="dataPoint"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public static async Task<bool> AddDevicePointAsync(Guid connectId, EquipDataPoint dataPoint)
        {
            if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
            {
                return await deviceManager.AddDataPointAsync(dataPoint);
            }

            throw new Exception("找不到该连接");
        }

        /// <summary>
        /// 删除一个采集点位
        /// </summary>
        /// <param name="connectId"></param>
        /// <param name="dataPoint"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public static async Task RemoveDevicePointAsync(Guid connectId, EquipDataPoint dataPoint)
        {
            if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
            {
                await deviceManager.RemoveDataPointAsync(dataPoint);
                SetStatusByPointId(dataPoint.Id, DataPointStatus.None);
            }
            else
            {
                throw new ApplicationException("找不到该连接");
            }
        }

        /// <summary>
        /// 设置更新当前点位的采集状态
        /// </summary>
        /// <param name="dataPointId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool SetStatusByPointId(Guid dataPointId, DataPointStatus status)
        {
            if (!DataPointStatusDict.ContainsKey(dataPointId)) return DataPointStatusDict.TryAdd(dataPointId, status);
            DataPointStatusDict[dataPointId] = status;
            return true;
        }
    }
}
