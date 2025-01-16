using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared
{
    public class EquipStatus
    {
        /// <summary>
        /// 设备连接ID
        /// </summary>
        public Guid ConnectId { get; set; }

        /// <summary>
        /// 设备连接状态
        /// </summary>
        public bool ConnectStatus { get; set; }

    }
}
