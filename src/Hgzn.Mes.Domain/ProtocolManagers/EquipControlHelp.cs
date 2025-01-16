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

        /// <summary>
        /// 判断是否是连接状态
        /// </summary>
        /// <param name="connectId"></param>
        /// <returns></returns>
        public static async Task<bool> IsConnectedAsync(Guid connectId)
        {
            if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
            {
                return await deviceManager.IsConnectedAsync();
            }

            return false;
        }
    }
}
