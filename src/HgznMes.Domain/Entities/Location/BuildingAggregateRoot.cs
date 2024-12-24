

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HgznMes.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace HgznMes.Domain.Entities.Location;

[Table("Build")]  // 设置表名为 Build
public class BuildingAggregateRoot :IAggregateRoot
{
    [Key]
    public Guid Id { get; protected set; }

    [Comment("建筑物名称")]
    public string Name { get; set; }
    
    [Column(TypeName = "nvarchar(50)")]
    [Comment("建筑物编号")]
    public string? Code { get; set; }

    [Column(TypeName = "nvarchar(255)")]
    [Comment("地址")]
    public string? Address { get; set; }

    [Comment("纬度")]  
    public double? Latitude { get; set; }

    [Comment("经度")]  
    public double? Longitude { get; set; }

    [Comment("建造日期")] 
    public DateTime? ConstructionDate { get; set; }

    public List<FloorEntity> Floors { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public int OrderNum { get; set; } = 100;
    
}