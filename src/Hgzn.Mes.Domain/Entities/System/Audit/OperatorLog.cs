using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Shared.Enums.Log;

namespace Hgzn.Mes.Domain.Entities.System.Audit;

public class OperatorLog:UniversalEntity,ICreationAudited
{
    /// <summary>
    /// 操作模块    
    ///</summary>
    [Description("Title")]
    public string? Title { get; set; }
    /// <summary>
    /// 操作类型 
    ///</summary>
    [Description("OperType")]
    public OperEnum OperType { get; set; }
    /// <summary>
    /// 请求方法 
    ///</summary>
    [Description("RequestMethod")]
    public string? RequestMethod { get; set; }
    /// <summary>
    /// 操作人员 
    ///</summary>
    [Description("OperUser")]
    public string? OperUser { get; set; }
    /// <summary>
    /// 操作Ip 
    ///</summary>
    [Description("OperIp")]
    public string? OperIp { get; set; }
    /// <summary>
    /// 操作地点 
    ///</summary>
    [Description("OperLocation")]
    public string? OperLocation { get; set; }
    /// <summary>
    /// 操作方法 
    ///</summary>
    [Description("Method")]
    public string? Method { get; set; }
    /// <summary>
    /// 请求参数 
    ///</summary>
    [Description("RequestParam")]
    public string? RequestParam { get; set; }
    /// <summary>
    /// 请求结果 
    ///</summary>
    [Description("RequestResult")]
    public string? RequestResult { get; set; }

    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
}