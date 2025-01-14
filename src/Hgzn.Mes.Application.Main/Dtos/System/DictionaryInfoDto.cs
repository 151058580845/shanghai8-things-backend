using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.System;

public class DictionaryInfoDto
{
}

public class DictionaryInfoReadDto : ReadDto
{
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public Guid? CreatorId { get; set; }
    public string? Remark { get; set; }
    public string? ListClass { get; set; }
    public string? CssClass { get; set; }
    public string DictType { get; set; } = string.Empty;
    public string? DictLabel { get; set; }
    public string DictValue { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public bool State { get; set; }
}

public class DictionaryInfoCreateDto : CreateDto
{
    public string? Remark { get; set; }
    public string? ListClass { get; set; }
    public string? CssClass { get; set; }
    public string DictType { get; set; } = string.Empty;
    public string? DictLabel { get; set; }
    public string DictValue { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? ParentId { get; set; }
    public bool State { get; set; }
}

public class DictionaryInfoUpdateDto : UpdateDto
{
    public string? Remark { get; set; }
    public string? ListClass { get; set; }
    public string? CssClass { get; set; }
    public string DictType { get; set; } = string.Empty;
    public string? DictLabel { get; set; }
    public string DictValue { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string? ParentId { get; set; }
    public bool State { get; set; }
}

public class DictionaryInfoQueryDto : PaginatedQueryDto
{
    public Guid? ParentId { get; set; }
    public string? DictLabel { get; set; }
    public string? DictValue { get; set; }
    public bool? State { get; set; }
}