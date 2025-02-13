using System.ComponentModel;
using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class EquipLedgerHistoryDto
{
    
}

public class EquipLedgerHistoryReadDto : ReadDto
{
    [Description("设备主键")]
    public Guid EquipId { get; set; }

    [Description("设备名称")]
    public string EquipCode { get; set; } = null!;

    [Description("所在房间")]
    public Guid? RoomId { get; set; }
    public string RoomName { get; set; } = null!;
    
    [Description("操作时间")]
    public DateTime OperatorTime { get; set; }

    /*
     * 搜索为1
     * 盘点为2
     * */
    [Description("操作类型")]
    public int Type { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
}

public class EquipLedgerHistoryCreateDto : CreateDto
{
    [Description("设备主键")]
    public Guid EquipId { get; set; }

    [Description("设备名称")]
    public string EquipCode { get; set; } = null!;

    [Description("所在房间")]
    public Guid? RoomId { get; set; }
    
    [Description("操作时间")]
    public long OperatorTime { get; set; }

    /*
     * 搜索为1
     * 盘点为2
     * */
    [Description("操作类型")]
    public int Type { get; set; }
}

public class EquipLedgerHistoryUpdateDto : UpdateDto
{
    
}

public class EquipLedgerHistoryQueryDto : PaginatedQueryDto
{
    [Description("设备主键")]
    public Guid? EquipId { get; set; }=Guid.Empty;

    [Description("设备名称")]
    public string? EquipCode { get; set; }

    [Description("所在房间")]
    public Guid? RoomId { get; set; }=Guid.Empty;
    
    [Description("操作时间")]
    public DateTime? OperatorTime { get; set; }

    /*
     * 搜索为1
     * 盘点为2
     * */
    [Description("操作类型")] public int? Type { get; set; } = 0;
}
    
