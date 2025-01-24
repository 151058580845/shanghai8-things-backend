using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipMaintenance
{
    public class EquipMaintenanceTask : UniversalEntity
    {
        public EquipMaintenanceTask()
        {
        }

        public EquipMaintenanceTask(Guid id)
        {
            Id = id;
        }

        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool State { get; set; }

        [Description("计划Id")]
        public Guid PlanId { get; set; }

        [Description("设备Id")]
        public Guid? EquipId { get; set; }

        [Description("任务状态")]
        public EquipmentMaintenanceStatus? Status { get; set; }

        [Description("责任人Id")]
        public Guid? ResponsibleUserId { get; set; }

        [Description("开始时间")]
        public DateTime? StartTime { get; set; }

        [Description("结束时间")]
        public DateTime? EndTime { get; set; }

        [Description("实际执行时间")]
        public DateTime? ActualData { get; set; }

        [Description("备注")]
        public string? Remark { get; set; }
    }
}
