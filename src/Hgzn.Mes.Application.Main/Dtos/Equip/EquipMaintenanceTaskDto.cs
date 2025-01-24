using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class EquipMaintenanceTaskReadDto : ReadDto
    {
        /// <summary>
        /// 计划Id
        /// </summary>
        public Guid PlanId { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid? EquipId { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public EquipmentMaintenanceStatus? Status { get; set; }

        /// <summary>
        /// 责任人Id
        /// </summary>
        public Guid? ResponsibleUserId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 实际执行时间
        /// </summary>
        public DateTime? ActualData { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        public bool State { get; set; }
    }

    public class EquipMaintenanceTaskCreateDto : CreateDto
    {
        public bool? State { get; set; } = true;

        /// <summary>
        /// 计划Id
        /// </summary>
        public Guid PlanId { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid? EquipId { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public EquipmentMaintenanceStatus? Status { get; set; }

        /// <summary>
        /// 责任人Id
        /// </summary>
        public Guid? ResponsibleUserId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 实际执行时间
        /// </summary>
        public DateTime? ActualData { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }

    public class EquipMaintenanceTaskUpdateDto : UpdateDto
    {
        public bool? State { get; set; } = true;

        /// <summary>
        /// 计划Id
        /// </summary>
        public Guid PlanId { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid? EquipId { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public EquipmentMaintenanceStatus? Status { get; set; }

        /// <summary>
        /// 责任人Id
        /// </summary>
        public Guid? ResponsibleUserId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 实际执行时间
        /// </summary>
        public DateTime? ActualData { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }

    public class EquipMaintenanceTaskQueryDto : PaginatedQueryDto
    {
        /// <summary>
        /// 计划Id
        /// </summary>
        public Guid? PlanId { get; set; }
        /// <summary>
        /// 设备Id
        /// </summary>
        public Guid? EquipId { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public EquipmentMaintenanceStatus? Status { get; set; }

        /// <summary>
        /// 责任人Id
        /// </summary>
        public Guid? ResponsibleUserId { get; set; }

        /// <summary>
        /// 实际执行时间
        /// </summary>
        public DateTime? ActualData { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
        public bool State { get; set; }
    }
}
