using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    /// <summary>
    /// 维护计划类型
    /// </summary>
    public enum EquipMaintenanceType
    {
        [Description("设备点检")]
        Check,
        [Description("设备保养")]
        Maintenance,
    }
}
