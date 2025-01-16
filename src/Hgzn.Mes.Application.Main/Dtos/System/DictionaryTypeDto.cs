using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.System;

public class DictionaryTypeDto
{
}

public class DictionaryTypeReadDto : ReadDto
{
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public Guid? CreatorId { get; set; }
    public string DictName { get; set; } = string.Empty;
    public string DictType { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public bool State { get; set; }
}

public class DictionaryTypeCreateDto : CreateDto
{
    public string DictName { get; set; } = string.Empty;
    public string DictType { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public bool State { get; set; }
}

public class DictionaryTypeUpdateDto : UpdateDto
{
    public string? DictName { get; set; } = string.Empty;
    public string? DictType { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public bool State { get; set; }
}

public class DictionaryTypeQueryDto : PaginatedQueryDto
{
    public string? DictName { get; set; }
    public string? DictType { get; set; }
    public Guid? ParentId { get; set; }
    public string? Remark { get; set; }
    public bool? State { get; set; }
}