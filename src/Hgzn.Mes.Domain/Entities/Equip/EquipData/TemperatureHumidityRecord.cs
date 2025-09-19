using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData
{
    [Description("温湿度记录")]
    public class TemperatureHumidityRecord : UniversalEntity, ICreationAudited
    {
        [Description("设备编号")]
        public string? EquipCode { get; set; }

        [Description("设备主键")]
        public Guid EquipId { get; set; }

        [Description("设备IP")]
        public string? IpAddress { get; set; }

        [Description("房间ID")]
        public Guid? RoomId { get; set; }

        [Description("房间名称")]
        public string? RoomName { get; set; }

        [Description("温度")]
        public float? Temperature { get; set; }

        [Description("湿度")]
        public float? Humidness { get; set; }

        [Description("记录时间")]
        public DateTime RecordTime { get; set; }

        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorLevel { get; set; }
    }
}
