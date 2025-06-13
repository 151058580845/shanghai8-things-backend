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
    
public static readonly Dept Shjdgcyjs = new Dept()
{
    Id = Guid.Parse("b8db8187-5034-4b8b-b1f9-0931e627ea58"), // 固定的 Guid 值
    DeptName = "上海机电工程研究所",
    DeptCode = "shjdgcyjs",
    OrderNum = 100,
    SoftDeleted = false,
    Leader = "No.8",
    Remark = "上海机电工程研究所"
};

public static readonly Dept Qishi = new Dept()
{
    Id = Guid.Parse("e9f9fe2c-5b7d-417b-b33d-3b1f0036a3ab"), // 固定的 Guid 值
    DeptName = "七室",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Shjdgcyjs.Id
};

public static readonly Dept Banzu1 = new Dept()
{
    Id = Guid.Parse("421d0735-2bbf-47f0-95b0-10e1a3a1fbcf"), // 固定的 Guid 值
    DeptName = "射频制导仿真班组",
    OrderNum = 100,
    SoftDeleted = false,
    ParentId = Qishi.Id
};

public static readonly Dept Banzu2 = new Dept()
{
    Id = Guid.Parse("62ab7f49-56bb-4f0b-8bcf-b9bbd2512c8f"), // 固定的 Guid 值
    DeptName = "红外制导仿真班组",
    OrderNum = 101,
    SoftDeleted = false,
    ParentId = Qishi.Id
};

   
    public static Dept[] Seeds { get; } =
    [
        Shjdgcyjs, Qishi, Banzu1, Banzu2
    ];
    
}