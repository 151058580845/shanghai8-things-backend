using Hgzn.Mes.Domain.Entities.System.Base;

namespace Hgzn.Mes.Domain.Entities.System.Code;

public class CodeRuleDefine : UniversalEntity, IOrder
{
    /// <summary>
    /// 编码规则
    /// </summary>
    public Guid CodeRuleId { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public string? CodeRuleType { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int OrderNum { get; set; }

    /// <summary>
    /// 流水长度
    /// </summary>
    public int? MaxFlow { get; set; }

    /// <summary>
    /// 当前流水号
    /// </summary>
    public int? NowFlow { get; set; }

    /// <summary>
    /// 确认当前编码被应用
    /// </summary>
    public bool? NowFlowIsSure { get; set; }

    /// <summary>
    /// 补位符
    /// </summary>
    public char? CodeCover { get; set; }

    /// <summary>
    /// 日期格式
    /// </summary>
    public string? DateFormat { get; set; }

    /// <summary>
    /// 常量
    /// </summary>
    public string? ConstantChar { get; set; }

    /// <summary>
    /// 元素键值
    /// </summary>
    public string? SourceKey { get; set; }

    /// <summary>
    /// 元素属性
    /// </summary>
    public string? SourceValue { get; set; }
}