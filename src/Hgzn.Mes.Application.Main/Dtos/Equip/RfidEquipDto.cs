using System.ComponentModel;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class RfidEquipReadDto
    {
        [Description("RFID的TID")]

        public string RfidTid { get; set; } = null!;

        [Description("设备编号")]

        public Guid EquipId { get; set; }

        public DateTime CreationTime { get; set; }

        public bool State { get; set; } = true;
    }
}
