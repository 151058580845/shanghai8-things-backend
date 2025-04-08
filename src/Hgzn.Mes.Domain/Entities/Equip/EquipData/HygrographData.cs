using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;

public class HygrographData : UniversalEntity
{
    
    [Description("设备编号")]
    public string? EquipCode { get; set; }
    
    [Description("设备主键")]
    public Guid EquipId { get; set; }
    
    [Description("设备Ip")]
    public string? IpAddress { get; set; }
    
    [Description("房间名称")]
    public string? RoomName { get; set; }
    
    [Description("温度")]
    public float Temperature { get; set; }
    
    [Description("湿度")]
    public float? Humidness { get; set; }
    
    [Description("发生时间")]
    public string CreateTime { get; set; }
    
}