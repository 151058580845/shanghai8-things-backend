using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class EquipMaintenancePlanReadDto : ReadDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 计划编码
        /// </summary>
        public string PlanCode { get; set; }
        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }
        /// <summary>
        /// 计划类型
        /// </summary>
        public string EquipMaintenancePlanType { get; set; }

        /// <summary>
        /// 执行次数
        /// </summary>
        public int FrequencyNumber { get; set; }
        /// <summary>
        /// 维护计划频率
        /// </summary>
        public string Frequency { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        public bool State { get; set; }
    }

    public class EquipMaintenancePlanCreateDto : CreateDto
    {
        /// <summary>
        /// 计划编码
        /// </summary>
        public string PlanCode { get; set; }
        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }
        /// <summary>
        /// 计划类型
        /// </summary>
        public string EquipMaintenancePlanType { get; set; }

        /// <summary>
        /// 执行次数
        /// </summary>
        public int? FrequencyNumber { get; set; }
        /// <summary>
        /// 维护计划频率
        /// </summary>
        public string? Frequency { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string? Status { get; set; }


        /// <summary>
        /// 计划设备表
        /// </summary>
        public List<Guid>? PlanEquipEntities { get; set; }
        /// <summary>
        /// 计划项目列表
        /// </summary>
        public List<Guid>? PlanItemEntities { get; set; }
    }

    public class EquipMaintenancePlanUpdateDto : UpdateDto
    {
        /// <summary>
        /// 计划编码
        /// </summary>
        public string PlanCode { get; set; }
        /// <summary>
        /// 计划名称
        /// </summary>
        public string PlanName { get; set; }
        /// <summary>
        /// 计划类型
        /// </summary>
        public string EquipMaintenancePlanType { get; set; }

        /// <summary>
        /// 执行次数
        /// </summary>
        public int? FrequencyNumber { get; set; }
        /// <summary>
        /// 维护计划频率
        /// </summary>
        public string? Frequency { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string? Status { get; set; }


        /// <summary>
        /// 计划设备表
        /// </summary>
        public List<Guid>? PlanEquipEntities { get; set; }
        /// <summary>
        /// 计划项目列表
        /// </summary>
        public List<Guid>? PlanItemEntities { get; set; }
    }

    public class EquipMaintenancePlanQueryDto : PaginatedQueryDto
    {
        /// <summary>
        /// 计划编码
        /// </summary>
        public string? PlanCode { get; set; }
        /// <summary>
        /// 计划名称
        /// </summary>
        public string? PlanName { get; set; }
        /// <summary>
        /// 计划类型
        /// </summary>
        public string? EquipMaintenancePlanType { get; set; }
    }
}
