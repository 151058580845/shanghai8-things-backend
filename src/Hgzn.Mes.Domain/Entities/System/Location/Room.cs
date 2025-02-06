using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Shared.Enum;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.System.Location;

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
    
    
    #region static

    public static Room room1 = new()
    {
        Id = Guid.Parse("065FB4E9-05C0-B88C-78FF-5539A928F71C"),
        ParentId = Floor.floor1.Id,
        CreationTime = DateTime.Now,
        Name = "101",
        Code = "101",
        Length = 10,
        Width = 11,
        Height = 12
    };
    public static Room room2 = new()
    {
        Id = Guid.Parse("C1B6B59D-BD55-3980-A0F8-AF9504AF4EEA"),
        ParentId = Floor.floor1.Id,
        CreationTime = DateTime.Now,
        Name = "102",
        Code = "102",
        Length = 120,
        Width = 11,
        Height = 12
    };
    #endregion

    public static Room[] Seeds { get; } = [room1,room2];
}