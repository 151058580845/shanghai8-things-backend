using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    /// <summary>
    /// 维护任务状态
    /// </summary>
    public enum EquipmentMaintenanceStatus
    {
        [Description("未开始")]
        NotStarted = 1,
        [Description("进行中")]
        InProgress = 2,
        [Description("已完成")]
        Completed = 3
    }
}
