using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Domain.Entities.Equip
{
    public class LocationLabel : UniversalEntity, IAudited
    {
        public string TagId { get; set; } = null!;

        public Guid? EquipLedgerId { get; set; }

        public EquipLedger? EquipLedger { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
