using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.System;

public class DepartmentDto
{
    
}

public class DepartmentReadDto : ReadDto
{
    /// <summary>
    /// 部门名称 
    ///</summary>
    public string DeptName { get; set; } = string.Empty;
    /// <summary>
    /// 部门编码 
    ///</summary>
    public string DeptCode { get; set; } = string.Empty;
    /// <summary>
    /// 负责人 
    ///</summary>
    public string? Leader { get; set; }
    /// <summary>
    /// 父级id 
    ///</summary>
    public Guid ParentId { get; set; }
    /// <summary>
    /// 描述 
    ///</summary>
    public string? Remark { get; set; }
    public bool State { get; set; }
    public int OrderNum { get; set; }
}

public class DepartmentCreateDto : CreateDto
{
    /// <summary>
    /// 部门名称 
    ///</summary>
    public string DeptName { get; set; } = string.Empty;
    /// <summary>
    /// 部门编码 
    ///</summary>
    public string DeptCode { get; set; } = string.Empty;
    /// <summary>
    /// 负责人 
    ///</summary>
    public string? Leader { get; set; }
    /// <summary>
    /// 父级id 
    ///</summary>
    public Guid ParentId { get; set; }
    /// <summary>
    /// 描述 
    ///</summary>
    public string? Remark { get; set; }
}

public class DepartmentUpdateDto : UpdateDto
{
    /// <summary>
    /// 部门名称 
    ///</summary>
    public string DeptName { get; set; } = string.Empty;
    /// <summary>
    /// 部门编码 
    ///</summary>
    public string DeptCode { get; set; } = string.Empty;
    /// <summary>
    /// 负责人 
    ///</summary>
    public string? Leader { get; set; }
    /// <summary>
    /// 父级id 
    ///</summary>
    public Guid ParentId { get; set; }
    /// <summary>
    /// 描述 
    ///</summary>
    public string? Remark { get; set; }
}

public class DepartmentQueryDto : PaginatedQueryDto
{
    /// <summary>
    /// 部门名称 
    ///</summary>
    public string? DeptName { get; set; } = string.Empty;
    /// <summary>
    /// 部门编码 
    ///</summary>
    public string? DeptCode { get; set; } = string.Empty;
    public bool? State { get; set; }
}