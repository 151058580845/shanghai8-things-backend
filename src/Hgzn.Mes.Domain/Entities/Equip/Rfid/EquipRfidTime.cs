namespace Hgzn.Mes.Domain.Entities.Equip.Rfid;

/// <summary>
/// 给redis判断用的实体，不要存入数据库
/// </summary>
public class EquipRfidTime
{
    public Guid EquipId { get; set; }
    public Guid RoomId { get; set; }
    public DateTime CreateTime { get; set; }
}