using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HgznMes.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace HgznMes.Domain.Entities.Location;

[Table("Floor")]
public class FloorEntity : IAggregateRoot
{
    [Key]
    public Guid Id { get; protected set; }

    [ForeignKey("BuildingAggregateRoot")]  // 外键设置到 BuildingAggregateRoot
    public Guid ParentId { get; set; }  // 对应建筑物的ID

    [Comment("楼层名称")]
    public string? Name { get; set; }

    [Comment("楼层编号")]
    public string? Code { get; set; }

    [Comment("楼层面积（平方米）")]
    public double? Area { get; set; }

    [Comment("房间数量")]
    public int? NumberOfRooms { get; set; }

    public List<RoomEntity> Rooms { get; set; }  // 楼层和房间是 1 对多关系
    
    public BuildingAggregateRoot? Building { get; set; }  // 每个 FloorEntity 关联一个 BuildingAggregateRoot

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public int OrderNum { get; set; } = 100;
}