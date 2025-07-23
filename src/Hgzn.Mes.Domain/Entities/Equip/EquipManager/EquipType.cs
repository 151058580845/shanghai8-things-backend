using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.System.Account;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager;

[Description("设备类型")]
public class EquipType : UniversalEntity, ICreationAudited, ISoftDelete, IState, ISeedsGeneratable
{
    [Description("设备类型编号")]
    public string TypeCode { get; set; } = null!;

    [Description("设备类型名称")]
    public string TypeName { get; set; } = null!;

    [Description("设备描述")]
    public string? Description { get; set; }

    [Description("设备图标")]
    public string? Icon { get; set; }

    [Description("排序号")]
    public int OrderNum { get; set; } = 0;

    [Description("上级Id")]
    public Guid? ParentId { get; set; }

    [Description("设备创建时间")]
    public DateTime CreationTime { get; set; }

    [Description("设备最后修改时间")]
    public DateTime? LastModificationTime { get; set; }

    [Description("绑定的协议")]
    public string? ProtocolEnum { get; set; }

    public bool State { get; set; }

    public Guid? CreatorId { get; set; }
    public int CreatorLevel { get; set; } = 0;

    /// <summary>
    /// 这个设备类型提前配置的可以采集的数据列表
    /// 采集数据列表
    /// </summary>
    // [Navigate(NavigateType.OneToMany, nameof(EquipDataEntity.TypeId))]
    // public List<EquipDataEntity>? EquipTypeDataEntities { get; set; }
    [Description("软删除")]
    public bool SoftDeleted { get; set; }

    public DateTime? DeleteTime { get; set; }
    #region static

    public static EquipType RfidReaderType = new()
    {
        TypeCode = "rfid-reader-1",
        TypeName = "Rfid读卡器1",
        Description = null,
        Icon = null,
        OrderNum = 0,
        ParentId = null,
        CreationTime = DateTime.Parse("2025/6/6 11:04:33"),
        LastModificationTime = null,
        ProtocolEnum = "RfidReader",
        State = true,
        CreatorId = User.DevUser.Id,
        SoftDeleted = false,
        DeleteTime = null,
        Id = new Guid("ece1d093-80b5-4343-8dbf-a9c330387c5e"),
    };

    public static EquipType RfidIssuerType = new()
    {
        TypeCode = "rfid-issuer-1",
        TypeName = "Rfid发卡器1",
        Description = null,
        Icon = null,
        OrderNum = 0,
        ParentId = null,
        CreationTime = DateTime.Parse("2025/6/9 15:07:43"),
        LastModificationTime = null,
        ProtocolEnum = "CardIssuer",
        State = true,
        CreatorId = new Guid("08e8bafc-1a6d-4ce8-a921-e95fae5ac56b"),
        SoftDeleted = false,
        DeleteTime = null,
        Id = new Guid("f365f053-5146-41dc-9a1b-59b104e992e9"),
    };

    public static EquipType RKType = new()
    {
        TypeCode = "RK",
        TypeName = "温湿度计",
        Description = null,
        Icon = null,
        OrderNum = 0,
        ParentId = null,
        CreationTime = DateTime.Parse("2025/6/9 15:07:43"),
        LastModificationTime = null,
        ProtocolEnum = "RKServer",
        State = true,
        CreatorId = new Guid("50397a79-9792-40d0-b27f-0baf608f720e"),
        SoftDeleted = false,
        DeleteTime = null,
        Id = new Guid("87460f4b-af0b-4f23-81b3-50554f928870"),
    };

    public static EquipType[] Seeds { get; } = new EquipType[] { RfidReaderType, RfidIssuerType, RKType };

    #endregion static
}