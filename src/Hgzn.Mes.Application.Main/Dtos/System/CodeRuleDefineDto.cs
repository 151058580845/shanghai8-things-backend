using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.System;

public class CodeRuleDefineDto
{
    
}

public class CodeRuleDefineReadDto : ReadDto
{
    /// <summary>
    /// 类型
    /// </summary>
    public string? CodeRuleType { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public string? OrderNum { get; set; }

    /// <summary>
    /// 流水长度
    /// </summary>
    public string? MaxFlow { get; set; }

    /// <summary>
    /// 当前流水号
    /// </summary>
    public string? NowFlow { get; set; }

    /// <summary>
    /// 补位符
    /// </summary>
    public string? CodeCover { get; set; }

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

public class CodeRuleDefineCreateDto : CreateDto
{
    /// <summary>
    /// 主键
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// 编码规则
    /// </summary>
    public Guid? CodeRuleId { get; set; }
    
    /// <summary>
    /// 类型
    /// </summary>
    public string? CodeRuleType { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int? OrderNum { get; set; }

    /// <summary>
    /// 流水长度
    /// </summary>
    public int? MaxFlow { get; set; }

    /// <summary>
    /// 当前流水号
    /// </summary>
    public int? NowFlow { get; set; }

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

public class CodeRuleDefineUpdateDto : UpdateDto
{
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
    /// 补位符
    /// </summary>
    public string? CodeCover { get; set; }

    /// <summary>
    /// 日期格式
    /// </summary>
    public int? DateFormat { get; set; }

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

public class CodeRuleDefineQueryDto : PaginatedQueryDto
{
    /// <summary>
    /// 编码规则
    /// </summary>
    public Guid? CodeRuleId { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public string? CodeRuleType { get; set; }

    /// <summary>
    /// 元素键值
    /// </summary>
    public string? SourceKey { get; set; }

    /// <summary>
    /// 元素属性
    /// </summary>
    public string? SourceValue { get; set; }
}