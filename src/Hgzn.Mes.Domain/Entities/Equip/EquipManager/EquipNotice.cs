using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager;

public class EquipNotice:UniversalEntity
{
    [Description("设备主键")]
    public Guid EquipId { get; set; }

    [Description("通知时间")]
    public DateTime SendTime { get; set; }
    
    [Description("设备通知类型")]
    public ConnStateType NoticeType { get; set; }
    
    [Description("通知标题")]
    public string? Title { get; set; }
    
    [Description("通知内容")]
    public string? Content { get; set; }
    
    [Description("通知描述")]
    public string? Description { get; set; }

}