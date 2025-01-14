using Hgzn.Mes.Application.Main.Dtos.Base;

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
}