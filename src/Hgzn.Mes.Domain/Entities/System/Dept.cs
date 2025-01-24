using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.System;

[Description("部门表")]
public class Dept : UniversalEntity, ISoftDelete, IAudited, IState, IOrder
{
    /// <summary>
    /// 部门名称 
    ///</summary>
    public string DeptName { get; set; } = string.Empty;
    /// <summary>
    /// 部门编码 
    ///</summary>
    public string DeptCode { get; set; } = string.Empty;
    /// <summary>
    /// 负责人 
    ///</summary>
    public string? Leader { get; set; }
    /// <summary>
    /// 父级id 
    ///</summary>
    public Guid ParentId { get; set; }
    /// <summary>
    /// 描述 
    ///</summary>
    public string? Remark { get; set; }
    public bool SoftDeleted { get; set; }
    public DateTime? DeleteTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public bool State { get; set; }
    public int OrderNum { get; set; }
}