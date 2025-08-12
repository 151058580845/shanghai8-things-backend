using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager;

public class EquipNotice : UniversalEntity, ICreationAudited
{
    [Description("设备主键")]
    public Guid EquipId { get; set; }

    [Description("通知时间")]
    public DateTime SendTime { get; set; }
    
    [Description("设备通知类型")]
    public EquipNoticeType NoticeType { get; set; }
    
    [Description("通知标题")]
    public string? Title { get; set; }
    
    [Description("通知内容")]
    public string? Content { get; set; }
    
    [Description("通知描述")]
    public string? Description { get; set; }

    [Description("试验系统ID")]
    public byte? SimuTestSysId { get; set; }
    public Guid? CreatorId { get; set; }
    public int CreatorLevel { get; set; } = 0;
    public DateTime CreationTime { get; set; }

}