
using Hgzn.Mes.Application.Main.Dtos.Base;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class LocationLabelReadDto : ReadDto
    {
        public string TagId { get; set; } = null!;

        public Guid? EquipLedgerId { get; set; }

        public EquipLedgerReadDto? EquipLedger { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }

    public class LocationLabelUpdateDto : UpdateDto
    {
        public string TagId { get; set; } = null!;
        public Guid? EquipLedgerId { get; set; }
    }

    public class LocationLabelCreateDto : CreateDto
    {
        public string TagId { get; set; } = null!;
        public Guid? EquipLedgerId { get; set; }
    }

    public class LocationLabelQueryDto : PaginatedQueryDto
    {
        public string? TagId { get; set; }
    }
}
