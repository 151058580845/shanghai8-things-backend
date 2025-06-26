using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager;

public class EquipLocationRecord : UniversalEntity, ICreationAudited
{
    [Description("设备主键")]
    public Guid EquipId { get; set; }

    [Description("设备名称")]
    public string? EquipName { get; set; }

    [Description("读卡器主键")]
    public Guid RfidEquipId { get; set; }

    [Description("读卡器名称")]
    public string? RfidEquipName { get; set; }

    [Description("定位时间")]
    public DateTime DateTime { get; set; }

    [Description("标签ID")]
    public string? Tid { get; set; }

    [Description("标签用户数据")]
    public string? Userdata { get; set; }

    [Description("Rfid连接ID")]
    public Guid ConnectId { get; set; }

    [Description("定位房间Id")]
    public Guid? RoomId { get; set; }

    [Description("定位房间名称")]
    public string? RoomName { get; set; }
    
    [Description("描述")]
    public string? Description { get; set; }
    public Guid? CreatorId { get; set; }
    public int CreatorLevel { get; set; } = 0;
    public DateTime CreationTime { get; set; }

}