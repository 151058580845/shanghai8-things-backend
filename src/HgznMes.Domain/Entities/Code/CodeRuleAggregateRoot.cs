using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HgznMes.Domain.Entities.Base;
using MES.SystemManagement.Domain.Entities;

namespace HgznMes.Domain.Entities.Code;

[Table("CodeRule")]
public class CodeRuleAggregateRoot: IAggregateRoot,  ISoftDelete
{
    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    [Column("Id")]
    public Guid Id { get; protected set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column("CreationTime")]
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 创建者ID
    /// </summary>
    [Column("CreatorId")]
    public Guid? CreatorId { get; set; }

    /// <summary>
    /// 最后修改者ID
    /// </summary>
    [Column("LastModifierId")]
    public Guid? LastModifierId { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    [Column("LastModificationTime")]
    public DateTime? LastModificationTime { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Column("OrderNum")]
    public int OrderNum { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [Column("State")]
    public bool State { get; set; }

    /// <summary>
    /// 规则名称
    /// </summary>
    [Column("CodeName")]
    public string CodeName { get; set; }

    /// <summary>
    /// 规则编号
    /// </summary>
    [Column("CodeNumber")]
    public string? CodeNumber { get; set; }

    /// <summary>
    /// 基础元素
    /// </summary>
    [Column("BasicDomain")]
    public string? BasicDomain { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column("Remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 规则列表
    /// </summary>
    public List<CodeRuleRuleEntity> CodeRuleRules { get; set; } = new List<CodeRuleRuleEntity>();

    /// <summary>
    /// 软删除标志
    /// </summary>
    [NotMapped] // EF 不会映射此字段
    public bool SoftDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [Column("DeleteTime")]
    public DateTime? DeleteTime { get; set; }
}