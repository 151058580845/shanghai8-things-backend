using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Domain.Entities.System.Code;

using System.Text.Json.Serialization;

namespace Hgzn.Mes.Application.Main.Dtos.System;

public class CodeRuleDto
{
}

public class CodeRuleReadDto : ReadDto
{
    /// <summary>
    ///
    /// </summary>
    public string? OrderNum { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public bool? State { get; set; }

    /// <summary>
    /// 规则名称
    /// </summary>
    public string? CodeName { get; set; }

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

    public List<CodeRuleDefine>? CodeRuleRules { get; set; }


}

public class CodeRuleCreateDto : CreateDto
{
    /// <summary>
    ///
    /// </summary>
    public int OrderNum { get; set; } = 100;

    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; } = true;

    /// <summary>
    /// 规则名称
    /// </summary>
    public string? CodeName { get; set; }

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

    //public CodeRuleDefineCreateDto? CodeRuleRulesTest { get; set; }
    public IEnumerable<CodeRuleDefineCreateDto>? CodeRuleRules { get; set; }

}

/// <summary>
/// 编码规则规则定义表创建实体表
///</summary>
public class CodeRuleEntityCreateCodeRuleDefines
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

public class CodeRuleUpdateDto : UpdateDto
{
    /// <summary>
    ///
    /// </summary>
    public int OrderNum { get; set; } = 100;

    /// <summary>
    /// 状态
    /// </summary>
    public bool State { get; set; } = true;

    /// <summary>
    /// 规则名称
    /// </summary>
    public string? CodeName { get; set; }

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

    public IEnumerable<CodeRuleDefineCreateDto>? codeRuleRules { get; set; } 
}

public class CodeRuleQueryDto : PaginatedQueryDto
{
    /// <summary>
    /// 状态
    /// </summary>
    public bool? State { get; set; }

    /// <summary>
    /// 规则名称
    /// </summary>
    public string? CodeName { get; set; }

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
}