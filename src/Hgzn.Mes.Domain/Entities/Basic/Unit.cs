using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Basic;

[Description("计量单位")]
public class Unit : UniversalEntity, IAudited
{
    [Description("单位编码")] public string Code { get; set; } = null!;
    [Description("单位名称")] public string Name { get; set; } = null!;
    [Description("是否是主单位")] public bool? IsMain { get; set; } = true;
    [Description("主单位Id")] public Guid? ParentId { get; set; }
    [Description("换算比例")] public decimal? Ratio { get; set; }
    [Description("备注")] public string? Remark { get; set; }
    public Guid? CreatorId { get; set; }
    public DateTime CreationTime { get; set; }
    public int CreatorLevel { get; set; } = 0;
    public Guid? LastModifierId { get; set; }
    public DateTime? LastModificationTime { get; set; }
}