using Hgzn.Mes.Application.Main.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class EquipMaintenanceRecordsReadDto : ReadDto
    {
        /// <summary>
        /// 维护任务Id
        /// </summary>
        public string? TaskId { get; set; }
        /// <summary>
        /// 维护开始日期
        /// </summary>
        public string? MaintenanceStartDate { get; set; }
        /// <summary>
        /// 维护完成日期
        /// </summary>
        public string? MaintenanceFinishDate { get; set; }
        /// <summary>
        /// 维护类型
        /// </summary>
        public string? MaintenanceType { get; set; }
        /// <summary>
        /// 维护结果
        /// </summary>
        public string? EquipMaintenanceResult { get; set; }
        /// <summary>
        /// 维护人员
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// 维护描述
        /// </summary>
        public string? Description { get; set; }
    }

    public class EquipMaintenanceRecordsCreateDto : CreateDto
    {
        /// <summary>
        /// 维护任务Id
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 维护开始日期
        /// </summary>
        public DateTime MaintenanceStartDate { get; set; }
        /// <summary>
        /// 维护完成日期
        /// </summary>
        public DateTime MaintenanceFinishDate { get; set; }
        /// <summary>
        /// 维护类型
        /// </summary>
        public int? MaintenanceType { get; set; }
        /// <summary>
        /// 维护结果
        /// </summary>
        public int? EquipMaintenanceResult { get; set; }
        /// <summary>
        /// 维护人员
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 维护描述
        /// </summary>
        public string? Description { get; set; }
    }

    public class EquipMaintenanceRecordsUpdateDto : UpdateDto
    {
        /// <summary>
        /// 维护任务Id
        /// </summary>
        public Guid TaskId { get; set; }
        /// <summary>
        /// 维护开始日期
        /// </summary>
        public DateTime MaintenanceStartDate { get; set; }
        /// <summary>
        /// 维护完成日期
        /// </summary>
        public DateTime MaintenanceFinishDate { get; set; }
        /// <summary>
        /// 维护类型
        /// </summary>
        public int? MaintenanceType { get; set; }
        /// <summary>
        /// 维护结果
        /// </summary>
        public int? EquipMaintenanceResult { get; set; }
        /// <summary>
        /// 维护人员
        /// </summary>
        public Guid? UserId { get; set; }
        /// <summary>
        /// 维护描述
        /// </summary>
        public string? Description { get; set; }
    }

    public class EquipMaintenanceRecordsQueryDto : PaginatedQueryDto
    {
        /// <summary>
        /// 维护类型
        /// </summary>
        public string? MaintenanceType { get; set; }
        /// <summary>
        /// 维护结果
        /// </summary>
        public string? EquipMaintenanceResult { get; set; }
        /// <summary>
        /// 维护描述
        /// </summary>
        public string? Description { get; set; }
    }
}
