using System.ComponentModel.DataAnnotations.Schema;
using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.Entities.Base.Audited;
using Microsoft.EntityFrameworkCore;

namespace HgznMes.Domain.Entities.Location;

public class Floor : UniversalEntity, IOrder, IAudited
{
    [ForeignKey("Building")]  // 外键设置到 BuildingAggregateRoot
    public Guid ParentId { get; set; }  // 对应建筑物的ID

    [Comment("楼层名称")]
    public string? Name { get; set; }

    [Comment("楼层编号")]
    public string? Code { get; set; }

    [Comment("楼层面积（平方米）")]
    public double? Area { get; set; }

    [Comment("房间数量")]
    public int? NumberOfRooms { get; set; }

    public ICollection<Room>? Rooms { get; set; } // 楼层和房间是 1 对多关系
    
    public Building? Building { get; set; }  // 每个 FloorEntity 关联一个 BuildingAggregateRoot

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public int OrderNum { get; set; } = 100;
}