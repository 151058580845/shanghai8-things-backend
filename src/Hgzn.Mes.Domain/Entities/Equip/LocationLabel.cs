using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared.Enum;

namespace Hgzn.Mes.Domain.Entities.Equip
{
    public class LocationLabel : UniversalEntity, IAudited
    {
        public string TagId { get; set; } = null!;

        public LabelType Type { get; set; }

        public Guid? EquipLedgerId { get; set; }

        public EquipLedger? EquipLedger { get; set; }

        public Guid? RoomId { get; set; }

        public Room? Room { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public int CreatorLevel { get; set; } = 0;
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
