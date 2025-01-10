using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Location;
using Hgzn.Mes.Domain.Entities.System.Base;
using Hgzn.Mes.Domain.Entities.System.Base.Audited;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager;

[Table("EquipLedger")]
[Description("设备台账")]
public class EquipLedger : UniversalEntity, ISoftDelete, IState, IOrder,IAudited
{
    [Description("设备编号")]
    public string EquipCode { get; set; }
    
    [Description( "设备名称")]
    public string EquipName { get; set; }
    
    [Description( "设备类型ID")]
    [ForeignKey(nameof(EquipTypeAggregate))]
    public Guid? TypeId { get; set; }
    
    [Description( "规格型号")]
    public string? Model { get; set; }
    
    [Description( "购置日期")]
    public DateTime? PurchaseDate { get; set; }
    
    [Description( "供应商ID")]
    public Guid? SupplierId { get; set; }
    
    [Description( "资产编号")]
    public string? AssetNumber { get; set; }
    
    [Description( "使用部门ID")]
    public Guid? DepartmentId { get; set; }
    
    [Description( "安装地点")]//若是rfidReader则不可为null
    public Guid? RoomId { get; set; }
    
    // [Navigate(NavigateType.OneToOne,nameof(RoomId))]
    public Room? Room { get; set; }
    
    [Description( "状态（在用/停用/报废）")]
    public EquipOperationStatus? EquipOperationStatus { get; set; }
    
    [Description( "有效期时间")]
    public DateTime? ValidityDate { get; set; }
    
    [Description( "设备资源集（需要做文档管理功能）")]
    public Guid? ResourceId { get; set; }
    
    [Description( "备注")]
    public string? Remark { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    // [Navigate(NavigateType.OneToOne, nameof(TypeId))]
    public EquipType? EquipTypeAggregate { get; set; }

    // /// <summary>
    // /// 部门
    // /// </summary>
    // [Navigate(NavigateType.OneToOne, nameof(DepartmentId))]
    // public DeptAggregateRoot? DeptAggregateRoot { get; set; }

    // /// <summary>
    // /// 维护计划列表
    // /// </summary>
    // [Navigate(typeof(MaintenancePlanEquipEntity), nameof(MaintenancePlanEquipEntity.EquipId),
    //     nameof(MaintenancePlanEquipEntity.PlanId))]
    // public List<EquipMaintenancePlanAggregateRoot> PlanAggregateRoots { get; set; }
    
    // /// <summary>
    // /// 采集数据列表
    // /// </summary>
    // [Navigate(NavigateType.OneToMany, nameof(EquipReceiveDataEntity.EquipId))]
    // public List<EquipReceiveDataEntity> ReceiveDataEntities { get; set; }
    /// <summary>
    /// 是否在移动中
    /// </summary>
    public bool HasMove { get; set; } = false;
    /// <summary>
    /// 设备当前的MAC地址
    /// </summary>
    public string? Mac { get; set; }
    /// <summary>
    /// 设备当前的IP地址
    /// </summary>
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// 设备重要级别
    /// </summary>
    public EquipLevelEnum? EquipLevel { get; set; } = EquipLevelEnum.Basic;
    // /// <summary>
    // /// 设备类型(默认为用户类型)
    // /// </summary>
    // public EquipType EquipType { get; set; } = EquipType.UserEquip;

    public bool State { get; set; } = true;
    public int OrderNum { get; set; } = 0;
    public bool SoftDeleted { get; set; }
    public DateTime? DeleteTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
}