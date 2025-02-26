using System.ComponentModel;

namespace Hgzn.Mes.Domain.Shared.Enum;

public enum Location
{
}

public enum LabelType
{
    Equip,
    Room,
    Unknow
}


/// <summary>
///     非房间类型写在过道之后
/// </summary>
public enum RoomPurpose
{
    [Description("试验室")]
    Laboratory,

    [Description("监控室")]
    MonitoringRoom,

    [Description("杂物间")]
    StorageRoom,

    [Description("办公室")]
    Office,

    [Description("会议室")]
    ConferenceRoom,

    [Description("休息室")]
    BreakRoom,

    [Description("仓库")]
    Warehouse,

    [Description("卫生间")]
    Restroom,

    [Description("过道")]
    Hallway,
    
    [Description("门")]
    Gate,
    
    [Description("楼梯")]
    Stair,

}