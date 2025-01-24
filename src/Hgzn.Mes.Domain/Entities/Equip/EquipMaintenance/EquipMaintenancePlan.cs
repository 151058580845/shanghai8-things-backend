using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipMaintenance
{
    public class EquipMaintenancePlan : UniversalEntity
    {
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool State { get; set; }
        public int OrderNum { get; set; }

        [Description("计划编码")]
        public string? PlanCode { get; set; }

        [Description("计划名称")]
        public string? PlanName { get; set; }

        [Description("计划类型")]
        public string? EquipMaintenancePlanType { get; set; }

        [Description("执行次数")]
        public int FrequencyNumber { get; set; }

        [Description("维护计划频率")]
        public Frequency? Frequency { get; set; }

        [Description("开始时间")]
        public DateTime StartTime { get; set; }

        [Description("结束时间")]
        public DateTime EndTime { get; set; }

        [Description("责任人Id")]
        public Guid? ResponsibleUserId { get; set; }

        [Description("状态")]
        public string? Status { get; set; }

        [Description("计划设备表")]
        public List<EquipLedger>? EquipLedgerAggregateRoots { get; set; }

        [Description("计划项目列表")]
        public List<EquipItems>? EquipItemsAggregateRoots { get; set; }
    }
}
