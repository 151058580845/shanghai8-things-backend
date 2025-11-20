using System.ComponentModel;
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Application.Main.Dtos.Equip;

public class EquipNoticeDto
{
    
}

public class EquipNoticeReadDto : ReadDto
{
    [Description("设备主键")]
    public Guid EquipId { get; set; }
    [Description("设备主键")]
    public string? EquipName { get; set; }
    [Description("设备主键")]
    public string? EquipCode { get; set; }
    [Description("通知时间")]
    public DateTime SendTime { get; set; }
    
    [Description("设备通知类型")]
    public EquipNoticeEnum NoticeType { get; set; }
    
    [Description("通知标题")]
    public string? Title { get; set; }
    
    [Description("通知内容")]
    public string? Content { get; set; }
    
    [Description("通知描述")]
    public string? Description { get; set; }

    public string? RoomName { get; set; }
}

public class EquipNoticeCreateDto : CreateDto
{
    [Description("设备主键")]
    public Guid EquipId { get; set; }

    [Description("通知时间")]
    public DateTime SendTime { get; set; }
    
    [Description("设备通知类型")]
    public EquipNoticeEnum NoticeType { get; set; }
    
    [Description("通知标题")]
    public string? Title { get; set; }
    
    [Description("通知内容")]
    public string? Content { get; set; }
    
    [Description("通知描述")]
    public string? Description { get; set; }
}

public class EquipNoticeUpdateDto : UpdateDto
{
    
}

public class EquipNoticeQueryDto : PaginatedQueryDto
{
    [Description("设备主键")]
    public Guid? EquipId { get; set; }
    
    [Description("设备名称")]
    public string? EquipName { get; set; }
    
    [Description("设备编号")]
    public string? EquipCode { get; set; }
    
    [Description("设备通知类型")]
    public EquipNoticeEnum? NoticeType { get; set; }
    
    [Description("通知标题")]
    public string? Title { get; set; }
    
    [Description("通知内容")]
    public string? Content { get; set; }
    
    [Description("通知描述")]
    public string? Description { get; set; }
}
    
