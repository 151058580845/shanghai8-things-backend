using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.System;

[Description("部门表")]
public class Dept : UniversalEntity, ISoftDelete, IAudited, IState, IOrder
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
    public bool SoftDeleted { get; set; }
    public DateTime? DeleteTime { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public bool State { get; set; } = true;
    public int OrderNum { get; set; }
    
public static readonly Dept Hg = new Dept()
{
    Id = Guid.Parse("b8db8187-5034-4b8b-b1f9-0931e627ea58"), // 固定的 Guid 值
    DeptName = "Hgzn",
    DeptCode = "hgzn",
    OrderNum = 100,
    SoftDeleted = false,
    Leader = "123",
    Remark = "如名所指"
};

public static readonly Dept Shenzhen = new Dept()
{
    Id = Guid.Parse("e9f9fe2c-5b7d-417b-b33d-3b1f0036a3ab"), // 固定的 Guid 值
    DeptName = "深圳总公司",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Hg.Id
};

public static readonly Dept Jiangxi = new Dept()
{
    Id = Guid.Parse("32f682ad-806b-4603-a5e4-8d18ea998b95"), // 固定的 Guid 值
    DeptName = "江西总公司",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Hg.Id
};

public static readonly Dept SzDept1 = new Dept()
{
    Id = Guid.Parse("d0b0f6c4-9ca2-46d0-bd94-6901d3488e48"), // 固定的 Guid 值
    DeptName = "研发部门",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Shenzhen.Id
};

public static readonly Dept SzDept2 = new Dept()
{
    Id = Guid.Parse("421d0735-2bbf-47f0-95b0-10e1a3a1fbcf"), // 固定的 Guid 值
    DeptName = "市场部门",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Shenzhen.Id
};

public static readonly Dept SzDept3 = new Dept()
{
    Id = Guid.Parse("62ab7f49-56bb-4f0b-8bcf-b9bbd2512c8f"), // 固定的 Guid 值
    DeptName = "测试部门",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Shenzhen.Id
};

public static readonly Dept SzDept4 = new Dept()
{
    Id = Guid.Parse("ef5a3f6e-84c4-49e9-b073-4ca179438d02"), // 固定的 Guid 值
    DeptName = "财务部门",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Shenzhen.Id
};

public static readonly Dept SzDept5 = new Dept()
{
    Id = Guid.Parse("7c88c0c1-d312-457a-b7f7-b1157771c937"), // 固定的 Guid 值
    DeptName = "运维部门",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Shenzhen.Id
};

public static readonly Dept JxDept1 = new Dept()
{
    Id = Guid.Parse("b6a34577-4be1-43f2-8a0b-dde24e13d7db"), // 固定的 Guid 值
    DeptName = "市场部门",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Jiangxi.Id
};

public static readonly Dept JxDept2 = new Dept()
{
    Id = Guid.Parse("f43d02fa-3d87-466e-b234-c0bc1db0219b"), // 固定的 Guid 值
    DeptName = "财务部门",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Jiangxi.Id
};

   
    public static Dept[] Seeds { get; } =
    [
        Hg, Shenzhen, Jiangxi, SzDept1, SzDept2, SzDept3, SzDept4, SzDept5, JxDept1, JxDept2
    ];
    
}