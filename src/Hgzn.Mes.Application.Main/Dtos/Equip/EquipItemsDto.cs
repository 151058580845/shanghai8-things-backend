using Hgzn.Mes.Application.Main.Dtos.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
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

    public class EquipItemsReadDto : ReadDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Description("编号")]
        public string? ItemCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string? ItemName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Description("类型")]
        public EquipMaintenanceType EquipMaintenanceType { get; set; }
        /// <summary>
        /// 标准
        /// </summary>
        [Description("标准")]
        public string? Standard { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [Description("内容")]
        public string? Content { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Description("备注")]
        public string? Remark { get; set; }
        /// <summary>
        /// 资源Id
        /// </summary>
        [Description("资源Id")]
        public Guid? ResourceId { get; set; }

        public bool State { get; set; }
    }


    public class EquipItemsQueryDto : PaginatedQueryDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string? ItemCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? ItemName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public EquipMaintenanceType? EquipMaintenanceType { get; set; }
        /// <summary>
        /// 标准
        /// </summary>
        public string? Standard { get; set; }
    }

    public class EquipItemsCreateDto : CreateDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string ItemCode { get; set; } = null!;
        /// <summary>
        /// 名称
        /// </summary>
        public string ItemName { get; set; } = null!;
        /// <summary>
        /// 类型
        /// </summary>
        public EquipMaintenanceType EquipMaintenanceType { get; set; }
        /// <summary>
        /// 标准
        /// </summary>
        public string? Standard { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string? Content { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 资源Id
        /// </summary>
        public Guid? ResourceId { get; set; }

        public bool State { get; set; }
    }

    public class EquipItemsUpdateDto : UpdateDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string? ItemName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public EquipMaintenanceType EquipMaintenanceType { get; set; }
        /// <summary>
        /// 标准
        /// </summary>
        public string? Standard { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string? Content { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 资源Id
        /// </summary>
        public Guid? ResourceId { get; set; }

        public bool State { get; set; }
    }
}
