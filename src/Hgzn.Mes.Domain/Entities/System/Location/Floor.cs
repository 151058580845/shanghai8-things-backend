using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.System.Account;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.System.Location;

public class Floor : UniversalEntity, IOrder, IAudited, ISeedsGeneratable
{
    [Description("对应建筑物的ID")]
    public Guid ParentId { get; set; }  // 对应建筑物的ID

    [Description("楼层名称")]
    public string Name { get; set; } = null!;

    [Description("楼层编号")]
    public string Code { get; set; } = null!;

    [Description("楼层面积（平方米）")]
    public double? Area { get; set; }

    [Description("房间数量")]
    public int? NumberOfRooms { get; set; }

    public List<Room>? Rooms { get; set; } // 楼层和房间是 1 对多关系

    public Building? Building { get; set; }  // 每个 FloorEntity 关联一个 Building

    public DateTime CreationTime { get; set; }

    public Guid? CreatorId { get; set; }

    public int CreatorLevel { get; set; } = 0;

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public int OrderNum { get; set; } = 100;

    #region static

    public static Floor Floor1 = new()
    {
        Id = Guid.Parse("6eb67743-2a45-6108-f261-816b7899fe24"),
        ParentId = Building.BaYuanBuilding.Id,
        CreationTime = DateTime.Now.ToLocalTime(),
        Name = "1楼",
        Code = "1",
        Area = 0,
        CreatorId = User.DevUser.Id,
    };

    public static Floor Floor2 = new()
    {
        Id = Guid.Parse("cacc6ace-e2cb-4d6d-855b-8fba3997ee5f"),
        ParentId = Building.BaYuanBuilding.Id,
        CreationTime = DateTime.Now.ToLocalTime(),
        Name = "2楼",
        Code = "2",
        Area = 0,
        CreatorId = User.DevUser.Id,
    };

    public static Floor Floor3 = new()
    {
        Id = Guid.Parse("ab353f57-cdf7-4485-b766-aed8c61522de"),
        ParentId = Building.BaYuanBuilding.Id,
        CreationTime = DateTime.Now.ToLocalTime(),
        Name = "3楼",
        Code = "3",
        Area = 0,
        CreatorId = User.DevUser.Id,
    };

    #endregion

    public static Floor[] Seeds { get; } = [Floor1, Floor2, Floor3];
}