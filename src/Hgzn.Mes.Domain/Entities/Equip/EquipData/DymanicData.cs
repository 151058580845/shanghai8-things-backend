
using Hgzn.Mes.Domain.Entities.Base;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData
{
    public class DymanicData : IncrementEntity
    {
        public Guid EquipId { get; set; }
        [Description("设备编号")]
        public string EquipCode { get; set; } = null!;

        [Description("设备名称")]
        public string EquipName { get; set; } = null!;

        public Guid EquipTypeId { get; set; }

        [Description("设备类型编号")]
        public string TypeCode { get; set; } = null!;

        [Description("设备类型名称")]
        public string TypeName { get; set; } = null!;

        public string Data { get; set; } = null!;

        public DateTime DateTime { get; set; }
    }
}
