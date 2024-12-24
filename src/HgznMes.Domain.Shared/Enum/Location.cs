using System.ComponentModel;

namespace HgznMes.Domain.Shared.Enum;

public enum Location
{
    
}

public enum RoomPurposeEnum
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
    Restroom
}