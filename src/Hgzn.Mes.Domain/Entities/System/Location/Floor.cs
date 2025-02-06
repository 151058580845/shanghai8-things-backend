using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hgzn.Mes.Domain.Entities.System.Location;

public class Floor : UniversalEntity, IOrder, IAudited
{
    [Description("对应建筑物的ID")]
    public Guid ParentId { get; set; }  // 对应建筑物的ID

    [Description("楼层名称")]
    public string? Name { get; set; }

    [Description("楼层编号")]
    public string? Code { get; set; }

    [Description("楼层面积（平方米）")]
    public double? Area { get; set; }

    [Description("房间数量")]
    public int? NumberOfRooms { get; set; }

    public ICollection<Room>? Rooms { get; set; } // 楼层和房间是 1 对多关系

    public Building? Building { get; set; }  // 每个 FloorEntity 关联一个 Building

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public int OrderNum { get; set; } = 100;
}