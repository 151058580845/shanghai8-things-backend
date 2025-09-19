using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enum;
using Hgzn.Mes.Domain.Shared.Enums;
using System.ComponentModel;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class EquipLedgerReadDto : ReadDto
{
    [Description("设备编号")] public string EquipCode { get; set; } = null!;

    [Description("设备名称")] public string EquipName { get; set; } = null!;

    [Description("设备类型ID")] public Guid? TypeId { get; set; }
    public string? TypeName { get; set; }
    [Description("规格型号")] public string? Model { get; set; }

    [Description("购置日期")] public DateTime? PurchaseDate { get; set; }

    [Description("资产编号")] public string? AssetNumber { get; set; }

    [Description("安装地点")] //若是rfidReader则不可为null
    public Guid? RoomId { get; set; }

    [Description("安装地点")] //若是rfidReader则不可为null
    public string? RoomName { get; set; }

    [Description("设备状态(正常/丢失/使用中)")] public string? DeviceStatus { get; set; }

    public string? DeviceStatusString { get; set; }

    [Description("有效期时间")] public DateTime? ValidityDate { get; set; }

    [Description("设备责任人")]
    public Guid? ResponsibleUserId { get; set; }

    [Description("设备责任人")]
    public string? ResponsibleUserName { get; set; }

    [Description("备注")] public string? Remark { get; set; }
    /// <summary>
    /// 设备类型为读写器时触发，楼层关键节点
    /// </summary>
    [Description("是否是关卡")]
    public bool? IsCustoms { get; set; } = false;

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
    public string? DeviceLevel { get; set; }
    public string? DeviceLevelString { get; set; }
    public bool State { get; set; } = true;
    public int OrderNum { get; set; } = 0;
    public bool SoftDeleted { get; set; }
    public DateTime? DeleteTime { get; set; }

    /// <summary>
    /// 是否计量设备
    /// </summary>
    public bool? IsMeasurementDevice { get; set; }
    /// <summary>
    /// 1为手动在设备中修改，2为读写器获取，3为通过url从rfid系统中获取
    /// </summary>
    [Description("房间标签来源")] //若是rfidReader则不可为null
    public int? RoomIdSourceType { get; set; } = 1;
}

public class EquipResponsibleUserReadDto : ReadDto
{

    [Description("设备责任人")]
    public Guid? ResponsibleUserId { get; set; }

    [Description("设备责任人")]
    public string? ResponsibleUserName { get; set; }
}

public class EquipLedgerSearchReadDto : ReadDto
{
    [Description("设备编号")] public string EquipCode { get; set; } = null!;

    [Description("设备名称")] public string EquipName { get; set; } = null!;

    [Description("设备类型ID")] public Guid? TypeId { get; set; }
    [Description("设备类型ID")] public string? TypeName { get; set; }
    [Description("规格型号")] public string? Model { get; set; }

    [Description("资产编号")] public string? AssetNumber { get; set; }

    [Description("安装地点")] public Guid? RoomId { get; set; }

    [Description("设备责任人")]
    public Guid? ResponsibleUserId { get; set; }

    [Description("设备责任人")]
    public string? ResponsibleUserName { get; set; }
}

public class EquipLocationLabelReadDto : ReadDto
{
    [Description("资产编号")] public string? AssetNumber { get; set; } = null!;

    [Description("设备名称")] public string? EquipName { get; set; } = null!;

    [Description("设备型号")] public string? Model { get; set; } = null!;

    [Description("设备类型")] public string? EquipTypeName { get; set; } = null!;

    [Description("标签Tid")] public string Tid { get; set; } = null!;

    [Description("设备责任人")] public Guid? ResponsibleUserId { get; set; }

    [Description("设备责任人")] public string? ResponsibleUserName { get; set; }
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
    /// 设备类型为读写器时触发，楼层关键节点
    /// </summary>
    [Description("是否是关卡")]
    public bool? IsCustoms { get; set; } = false;

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
    /// 设备重要度(重要,一般,普通)
    /// </summary>
    public string? DeviceLevel { get; set; }

    /// <summary>
    /// 有效期时间
    /// </summary>
    public DateTime? ValidityDate { get; set; }

    [Description("设备责任人")]
    public Guid? ResponsibleUserId { get; set; }

    [Description("设备责任人")]
    public string? ResponsibleUserName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    public bool State { get; set; } = true;
    /// <summary>
    /// 设备当前的IP地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 是否计量设备
    /// </summary>
    public bool? IsMeasurementDevice { get; set; }
    /// <summary>
    /// 1为手动在设备中修改，2为读写器获取，3为通过url从rfid系统中获取
    /// </summary>
    [Description("房间标签来源")] //若是rfidReader则不可为null
    public int? RoomIdSourceType { get; set; } = 1;
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
    /// 设备类型为读写器时触发，楼层关键节点
    /// </summary>
    [Description("是否是关卡")]
    public bool? IsCustoms { get; set; } = false;

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
    /// 设备重要度(重要,一般,普通)
    /// </summary>
    public string? DeviceLevel { get; set; }

    /// <summary>
    /// 有效期时间
    /// </summary>
    public DateTime? ValidityDate { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
    /// <summary>
    /// 设备当前的IP地址
    /// </summary>
    public string? IpAddress { get; set; }

    [Description("设备责任人")]
    public Guid? ResponsibleUserId { get; set; }

    [Description("设备责任人")]
    public string? ResponsibleUserName { get; set; }

    [Description("是否计量设备")]
    public bool? IsMeasurementDevice { get; set; }
    /// <summary>
    /// 1为手动在设备中修改，2为读写器获取，3为通过url从rfid系统中获取
    /// </summary>
    [Description("房间标签来源")] //若是rfidReader则不可为null
    public int? RoomIdSourceType { get; set; } = 1;
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
    /// 设备类型为读写器时触发，楼层关键节点
    /// </summary>
    [Description("是否是关卡")]
    public bool? IsCustoms { get; set; } = false;

    /// <summary>
    /// 设备资源集（需要做文档管理功能）
    /// </summary>
    public Guid? ResourceId { get; set; }

    [Description("设备责任人")]
    public Guid? ResponsibleUserId { get; set; }

    [Description("设备责任人")]
    public string? ResponsibleUserName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; set; }
    /// <summary>
    /// 1为手动在设备中修改，2为读写器获取，3为手持枪获取，4为通过url从rfid系统中获取
    /// </summary>
    [Description("房间标签来源")] //若是rfidReader则不可为null
    public int? RoomIdSourceType { get; set; } = 1;
}

public class EquipLedgerQueryDto : PaginatedTimeQueryDto
{
    /// <summary>
    /// 设备类型编号
    /// </summary>
    public string? EquipCode { get; set; }

    /// <summary>
    /// 设备名称
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

    /// <summary>
    /// 设备责任人
    /// </summary>
    public Guid? ResponsibleUserId { get; set; }

    /// <summary>
    /// 资产编号
    /// </summary>
    public string? AssetNumber { get; set; }

    /// <summary>
    /// 模糊匹配设备名称和型号
    /// </summary>
    public string? Query { get; set; }

    public int? BindingTagCount { get; set; }

    public bool? NoRfidDevice { get; set; }

}

/// <summary>
/// 关键设备工作时长导出DTO
/// </summary>
public class KeyEquipWorkingHoursExportDto
{
    /// <summary>
    /// 本地化资产编号
    /// </summary>
    public string? AssetNumber { get; set; }

    /// <summary>
    /// 资产名称
    /// </summary>
    public string EquipName { get; set; } = null!;

    /// <summary>
    /// 设备规格型号
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// 安装地点
    /// </summary>
    public string? RoomName { get; set; }

    /// <summary>
    /// 是否计量
    /// </summary>
    public bool? IsMeasurementDevice { get; set; }

    /// <summary>
    /// 设备责任人
    /// </summary>
    public string? ResponsibleUserName { get; set; }

    /// <summary>
    /// 设备工作时长(小时)
    /// </summary>
    public decimal WorkingHours { get; set; }
}