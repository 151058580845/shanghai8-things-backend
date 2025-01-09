using System.ComponentModel;
using HgznMes.Domain.Entities.System.Base;
using HgznMes.Domain.Entities.System.Base.Audited;
using HgznMes.Domain.Shared.Enum;

namespace HgznMes.Domain.Entities.System.Location;

public class Room : UniversalEntity, IOrder, IAudited
{
    public Guid ParentId { get; set; }  // 外键，关联 FloorEntity

    [Description("房间名称")]
    public string? Name { get; set; }

    [Description("房间编号")]
    public string? Code { get; set; }

    [Description("房间长度（米）")]
    public double Length { get; set; }

    [Description("房间宽度（米）")]
    public double Width { get; set; }

    [Description("房间高度（米）")]
    public double Height { get; set; }

    [Description("房间用途")]
    public RoomPurpose Purpose { get; set; }

    [Description("属于同一个房间组")]
    public Guid GroupId { get; set; }

    // One-to-one relationship with FloorEntity
    public Floor? Floor { get; set; }  // 房间对应的楼层

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    /// <summary>
    /// 排序字段
    /// </summary>
    public int OrderNum { get; set; }
}