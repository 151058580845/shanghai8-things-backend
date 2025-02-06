using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.System.Monitor;

public class LoginLog:UniversalEntity,ICreationAudited
{
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    
    /// <summary>
    /// 登录用户 
    ///</summary>
    [Description("LoginUser")]
    public string? LoginUser { get; set; }
    /// <summary>
    /// 登录地点 
    ///</summary>
    [Description( "LoginLocation")]
    public string? LoginLocation { get; set; }
    /// <summary>
    /// 登录Ip 
    ///</summary>
    [Description( "LoginIp")]
    public string? LoginIp { get; set; }
    /// <summary>
    /// 浏览器 
    ///</summary>
    [Description( "Browser")]
    public string? Browser { get; set; }
    /// <summary>
    /// 操作系统 
    ///</summary>
    [Description( "Os")]
    public string? Os { get; set; }
    /// <summary>
    /// 登录信息 
    ///</summary>
    [Description( "LogMsg")]
    public string? LogMsg { get; set; }
}