using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class RfidEquipReadDto : UniversalEntity
    {
        [Description("RFID的TID")]

        public string RfidTid { get; set; }

        [Description("设备编号")]

        public Guid EquipId { get; set; }

        public DateTime CreationTime { get; set; }

        public bool State { get; set; } = true;
    }
}
