using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HgznMes.Domain.Entities.Base;
using HgznMes.Domain.Shared.Enum;
using Microsoft.EntityFrameworkCore;

namespace HgznMes.Domain.Entities.Location;

[Table("Room")]
public class RoomEntity : IAggregateRoot
{
    [Key]
    public Guid Id { get; set; }

    public Guid ParentId { get; set; }  // 外键，关联 FloorEntity

    [Comment("房间名称")]
    public string? Name { get; set; }

    [Comment("房间编号")]
    public string? Code { get; set; }

    [Comment("房间长度（米）")]
    public double Length { get; set; }

    [Comment("房间宽度（米）")]
    public double Width { get; set; }

    [Comment("房间高度（米）")]
    public double Height { get; set; }

    [Comment("房间用途")]
    public RoomPurposeEnum Purpose { get; set; }

    [Comment("属于同一个房间组")]
    public Guid GroupId { get; set; }

    // One-to-one relationship with FloorEntity
    public FloorEntity? Floor { get; set; }  // 房间对应的楼层

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public int OrderNum { get; set; }  // 排序字段
}