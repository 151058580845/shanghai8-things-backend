using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.System.Code;

[Table("CodeRuleDefine")]
public class CodeRuleDefine : UniversalEntity, IOrder
{
    [Description("编码规则")]
    public Guid CodeRuleId { get; set; }

    [Description("类型")]
    public string? CodeRuleType { get; set; }

    [Description("排序号")]
    public int OrderNum { get; set; }

    [Description("流水长度")]
    public int? MaxFlow { get; set; }

    [Description("当前流水号")]
    public int? NowFlow { get; set; }

    [Description("确认当前编码被应用")]
    public bool? NowFlowIsSure { get; set; }

    [Description("补位符")]
    public char? CodeCover { get; set; }

    [Description("日期格式")]
    public string? DateFormat { get; set; }

    [Description("常量")]
    public string? ConstantChar { get; set; }

    [Description("元素键值")]
    public string? SourceKey { get; set; }

    [Description("元素属性")]
    public string? SourceValue { get; set; }
}