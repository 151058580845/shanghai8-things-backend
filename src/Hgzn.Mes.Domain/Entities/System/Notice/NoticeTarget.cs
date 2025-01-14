using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Domain.Entities.System.Notice;

[Table("NoticeTarget")]
public class NoticeTarget : UniversalEntity
{
    [Description("通知id")]
    public Guid NoticeId { get; set; }

    [Description("目标类型")]
    public NoticeTargetType NoticeTargetType { get; set; }

    [Description("目标Id")]
    public Guid NoticeObjectId { get; set; }

    [Description("通知发送时间")]
    public DateTime NoticeTime { get; set; } = DateTime.Now;

    [Description("阅读状态")]
    public bool IsRead { get; set; } = false;
}