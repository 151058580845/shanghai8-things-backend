using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    /// <summary>
    /// 设备维护结果
    /// </summary>
    public enum EquipMaintenanceResult
    {
        [Description("报废")]
        Scrapped = 1,
        [Description("修复")]
        Repair = 2,
        [Description("成功")]
        Success = 3
    }
}
