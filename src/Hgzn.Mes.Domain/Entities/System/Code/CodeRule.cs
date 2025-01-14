using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.System.Code;

[Table("CodeRule")]
public class CodeRule : UniversalEntity, ISoftDelete, IState, IOrder
{
    [Description("创建时间")]
    public DateTime CreationTime { get; set; }

    [Description("创建者ID")]
    public Guid? CreatorId { get; set; }

    [Description("最后修改者ID")]
    public Guid? LastModifierId { get; set; }

    [Description("最后修改时间")]
    public DateTime? LastModificationTime { get; set; }

    [Description("排序")]
    public int OrderNum { get; set; }

    [Description("状态")]
    public bool State { get; set; }

    [Description("规则名称")]
    public string CodeName { get; set; } = null!;

    [Description("规则编号")]
    public string? CodeNumber { get; set; }

    [Description("基础元素")]
    public string? BasicDomain { get; set; }

    [Description("备注")]
    public string? Remark { get; set; }

    [Description("规则列表")]
    public ICollection<CodeRuleDefine>? CodeRuleRules { get; set; }

    #region delete filter

    [Description("软删除标志")]
    public bool SoftDeleted { get; set; } = false;

    [Description("删除时间")]
    public DateTime? DeleteTime { get; set; } = null;

    #endregion delete filter
}