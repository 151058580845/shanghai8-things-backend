using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Audit;

public class LoginLog : UniversalEntity, ISoftDelete, IAudited
{
    public bool SoftDeleted { get; set; }
    public DateTime? DeleteTime { get; set; }
    public Guid? CreatorId { get; set; }
    public int CreatorLevel { get; set; } = 0;
    public DateTime CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    
    /// <summary>
    /// 登录用户 
    ///</summary>
    public string? LoginUser { get; set; }
    /// <summary>
    /// 登录地点 
    ///</summary>
    public string? LoginLocation { get; set; }
    /// <summary>
    /// 登录Ip 
    ///</summary>
    public string? LoginIp { get; set; }
    /// <summary>
    /// 浏览器 
    ///</summary>
    public string? Browser { get; set; }
    /// <summary>
    /// 操作系统 
    ///</summary>
    public string? Os { get; set; }
    /// <summary>
    /// 登录信息 
    ///</summary>
    public string? LogMsg { get; set; }
}