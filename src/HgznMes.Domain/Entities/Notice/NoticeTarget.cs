using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.Shared.Enums;

namespace HgznMes.Domain.Entities.Notice;

public class NoticeTarget : UniversalEntity
{

    /// <summary>
    /// 通知id 
    /// </summary>
    public Guid NoticeId { get; set; }

    /// <summary>
    /// 目标类型
    /// </summary>
    public NoticeTargetType NoticeTargetType { get; set; }

    /// <summary>
    /// 目标Id
    /// </summary>
    public Guid NoticeObjectId { get; set; }

    /// <summary>
    /// 通知发送时间
    /// </summary>
    public DateTime NoticeTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 阅读状态
    /// </summary>
    public bool IsRead { get; set; } = false;
}