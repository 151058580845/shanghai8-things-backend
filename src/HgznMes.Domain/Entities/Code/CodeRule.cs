using HgznMes.Domain.Entities.Base;
using MES.SystemManagement.Domain.Entities;

namespace HgznMes.Domain.Entities.Code;

public class CodeRule : UniversalEntity, ISoftDelete, IState, IOrder
{

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 创建者ID
    /// </summary>
    public Guid? CreatorId { get; set; }

    /// <summary>
    /// 最后修改者ID
    /// </summary>
    public Guid? LastModifierId { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; }

    /// <summary>
    /// 规则名称
    /// </summary>
    public string CodeName { get; set; } = null!;

    /// <summary>
    /// 规则编号
    /// </summary>
    public string? CodeNumber { get; set; }

    /// <summary>
    /// 基础元素
    /// </summary>
    public string? BasicDomain { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 规则列表
    /// </summary>
    public ICollection<CodeRuleDefine>? CodeRuleRules { get; set; }

    #region delete filter

    public bool SoftDeleted { get; set; } = false;
    public DateTime? DeleteTime { get; set; } = null;

    #endregion delete filter
}