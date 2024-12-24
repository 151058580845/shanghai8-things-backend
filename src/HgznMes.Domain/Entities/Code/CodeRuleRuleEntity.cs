using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MES.SystemManagement.Domain.Entities;

[Table("CodeRuleEntity")]
public class CodeRuleRuleEntity 
{
    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    [Column("Id")]
    public Guid Id { get; protected set; }

    /// <summary>
    /// 编码规则
    /// </summary>
    [Column("CodeRuleId")]
    public Guid CodeRuleId { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [Column("CodeRuleType")]
    public string? CodeRuleType { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    [Column("OrderNum")]
    public int OrderNum { get; set; }

    /// <summary>
    /// 流水长度
    /// </summary>
    [Column("MaxFlow")]
    public int? MaxFlow { get; set; }

    /// <summary>
    /// 当前流水号
    /// </summary>
    [Column("NowFlow")]
    public int? NowFlow { get; set; }

    /// <summary>
    /// 确认当前编码被应用
    /// </summary>
    [Column("NowFlowIsSure")]
    public bool? NowFlowIsSure { get; set; }

    /// <summary>
    /// 补位符
    /// </summary>
    [Column("CodeCover")]
    public char? CodeCover { get; set; }

    /// <summary>
    /// 日期格式
    /// </summary>
    [Column("DateFormat")]
    public string? DateFormat { get; set; }

    /// <summary>
    /// 常量
    /// </summary>
    [Column("ConstantChar")]
    public string? ConstantChar { get; set; }

    /// <summary>
    /// 元素键值
    /// </summary>
    [Column("SourceKey")]
    public string? SourceKey { get; set; }

    /// <summary>
    /// 元素属性
    /// </summary>
    [Column("SourceValue")]
    public string? SourceValue { get; set; }
}