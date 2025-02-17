using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums;
using System.ComponentModel;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class EquipLedgerDto
{
}

public class EquipLedgerReadDto : ReadDto
{
    [Description("设备编号")] public string EquipCode { get; set; } = null!;

    [Description("设备名称")] public string EquipName { get; set; } = null!;

    [Description("设备类型ID")] public Guid? TypeId { get; set; }
    public string TypeName { get; set; }
    [Description("规格型号")] public string? Model { get; set; }

    [Description("购置日期")] public DateTime? PurchaseDate { get; set; }

    [Description("资产编号")] public string? AssetNumber { get; set; }

    [Description("安装地点")] //若是rfidReader则不可为null
    public Guid? RoomId { get; set; }

    [Description("安装地点")] //若是rfidReader则不可为null
    public string? RoomName { get; set; }

    [Description("设备状态(正常/丢失/使用中)")] public DeviceStatus? DeviceStatus { get; set; }

    [Description("有效期时间")] public DateTime? ValidityDate { get; set; }

    [Description("备注")] public string? Remark { get; set; }

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

    public bool State { get; set; } = true;
    public int OrderNum { get; set; } = 0;
    public bool SoftDeleted { get; set; }
    public DateTime? DeleteTime { get; set; }
}

public class EquipLedgerSearchReadDto : ReadDto
{
    public Guid Id { get; set; }
    [Description("设备编号")] public string EquipCode { get; set; } = null!;

    [Description("设备名称")] public string EquipName { get; set; } = null!;

    [Description("设备类型ID")] public Guid? TypeId { get; set; }
    [Description("设备类型ID")] public string? TypeName { get; set; }
    [Description("规格型号")] public string? Model { get; set; }

    [Description("资产编号")] public string? AssetNumber { get; set; }

    [Description("安装地点")] public Guid? RoomId { get; set; }
}

public class EquipLedgerTestReadDto : ReadDto
{
    [Description("系统名称")] public string TestName { get; set; }

    [Description("正常设备列表")] public List<EquipLedgerReadDto> NormalList { get; set; } = null!;

    [Description("空闲设备列表")] public List<EquipLedgerReadDto> FreeList { get; set; } = null!;

    [Description("离线设备列表")] public List<EquipLedgerReadDto> LeaveList { get; set; } = null!;

    [Description("健康设备列表")] public List<EquipLedgerReadDto> HealthList { get; set; } = null!;

    [Description("较好设备列表")] public List<EquipLedgerReadDto> BetterList { get; set; } = null!;

    [Description("故障设备列表")] public List<EquipLedgerReadDto> ErrorList { get; set; } = null!;
}

public class EquipLedgerCreateDto : CreateDto
{
    /// <summary>
    /// 设备编号
    /// </summary>
    public string EquipCode { get; set; } = null!;

    /// <summary>
    /// 设备名称
    /// </summary>
    public string EquipName { get; set; } = null!;

    /// <summary>
    /// 设备类型ID
    /// </summary>
    public Guid? TypeId { get; set; }

    /// <summary>
    /// 规格型号
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 购置日期
    /// </summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// 资产编号
    /// </summary>
    public string? AssetNumber { get; set; }

    /// <summary>
    /// 安装地点
    /// </summary>
    public Guid? RoomId { get; set; }

    /// <summary>
    /// 设备状态(正常/丢失/使用中)
    /// </summary>
    public string? DeviceStatus { get; set; }

    /// <summary>
    /// 有效期时间
    /// </summary>
    public DateTime? ValidityDate { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

public class EquipLedgerUpdateDto : UpdateDto
{
    /// <summary>
    /// 设备编号
    /// </summary>
    public string EquipCode { get; set; } = null!;

    /// <summary>
    /// 设备名称
    /// </summary>
    public string EquipName { get; set; } = null!;

    /// <summary>
    /// 设备类型ID
    /// </summary>
    public Guid? TypeId { get; set; }

    /// <summary>
    /// 规格型号
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 购置日期
    /// </summary>
    public DateTime? PurchaseDate { get; set; }


    /// <summary>
    /// 资产编号
    /// </summary>
    public string? AssetNumber { get; set; }


    /// <summary>
    /// 安装地点
    /// </summary>
    public Guid? RoomId { get; set; }

    /// <summary>
    /// 设备状态(正常/丢失/使用中)
    /// </summary>
    public string? DeviceStatus { get; set; }

    /// <summary>
    /// 有效期时间
    /// </summary>
    public DateTime? ValidityDate { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

public class EquipLedgerAppUpdateDto : UpdateDto
{
    /// <summary>
    /// 设备主键
    /// </summary>
    public string EquipId { get; set; } = null!;

    /// <summary>
    /// 房间主键
    /// </summary>
    public string RoomId { get; set; } = null!;

    /// <summary>
    /// 设备类型ID
    /// </summary>
    public Guid? TypeId { get; set; }

    /// <summary>
    /// 规格型号
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 购置日期
    /// </summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// 供应商ID
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// 资产编号
    /// </summary>
    public string? AssetNumber { get; set; }

    /// <summary>
    /// 使用部门ID
    /// </summary>
    public Guid? DepartmentId { get; set; }

    /// <summary>
    /// 安装地点
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 状态（在用/停用/报废）
    /// </summary>
    public EquipOperationStatus? EquipOperationStatus { get; set; }

    /// <summary>
    /// 有效期时间
    /// </summary>
    public DateTime? ValidityDate { get; set; }

    /// <summary>
    /// 设备资源集（需要做文档管理功能）
    /// </summary>
    public Guid? ResourceId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; set; }
}

public class EquipLedgerQueryDto : PaginatedQueryDto
{
    /// <summary>
    /// 设备类型编号
    /// </summary>
    public string? EquipCode { get; set; }

    /// <summary>
    /// 设备类型名称
    /// </summary>
    public string? EquipName { get; set; }

    /// <summary>
    /// 设备状态
    /// </summary>
    public bool? State { get; set; }

    /// <summary>
    /// 设备类型
    /// </summary>
    public Guid? TypeId { get; set; }

    /// <summary>
    /// 房间Id
    /// </summary>
    public Guid? RoomId { get; set; }
}