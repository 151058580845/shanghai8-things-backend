using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.System.Location;

[Table("Building")]
public class Building : UniversalEntity, IAudited, ISoftDelete, IOrder
{
    [Description("建筑物名称")]
    public string Name { get; set; } = null!;

    [Column(TypeName = "varchar(50)")]
    [Description("建筑物编号")]
    public string? Code { get; set; }

    [Column(TypeName = "varchar(255)")]
    [Description("地址")]
    public string? Address { get; set; }

    [Description("纬度")]
    public double? Latitude { get; set; }

    [Description("经度")]
    public double? Longitude { get; set; }

    [Description("建造日期")]
    public DateTime? ConstructionDate { get; set; }

    public ICollection<Floor>? Floors { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public int OrderNum { get; set; } = 100;

    #region delete filter

    public bool SoftDeleted { get; set; } = false;
    public DateTime? DeleteTime { get; set; } = null;

    #endregion delete filter

}