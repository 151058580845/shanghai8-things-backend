using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HgznMes.Domain.Shared.Enum;

namespace HgznMes.Domain.Entities.Notice;

[Table("NoticeTargets")]
public class NoticeTargetsEntity
{
    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    public Guid Id { get; protected set; }

    /// <summary>
    /// 通知id 
    /// </summary>
    [ForeignKey("Notice")]
    public Guid NoticeId { get; set; }

    /// <summary>
    /// 目标类型
    /// </summary>
    [Column("NoticeTarget")]
    public NoticeTargetEnum NoticeTarget { get; set; }

    /// <summary>
    /// 目标Id
    /// </summary>
    [Column("NoticeObjectId")]
    public Guid NoticeObjectId { get; set; }

    /// <summary>
    /// 通知发送时间
    /// </summary>
    [Column("NoticeTime")]
    public DateTime NoticeTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 阅读状态
    /// </summary>
    [Column("IsRead")]
    public bool IsRead { get; set; } = false;
}