using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hgzn.Mes.Domain.Entities.System.Location;

public class Building : UniversalEntity, IAudited, ISoftDelete, IOrder, ISeedsGeneratable
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

    public List<Floor>? Floors { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public int CreatorLevel { get; set; } = 0;

    public Guid? LastModifierId { get; set; }

    public int OrderNum { get; set; } = 100;

    #region delete filter

    public bool SoftDeleted { get; set; } = false;
    public DateTime? DeleteTime { get; set; } = null;

    #endregion delete filter
    
    #region static

    public static Building BaYuanBuilding = new Building()
    {
        Id = Guid.Parse("4f125e25-3c84-4740-ace2-a7d8d7ca61a1"),
        CreationTime = DateTime.Now,
        Name = "上海机电工程研究所",
        Code = "2#",
        Address = "",
        Latitude = 0,
        Longitude = 0,
    };

    #endregion

    public static Building[] Seeds { get; } = [BaYuanBuilding];
}