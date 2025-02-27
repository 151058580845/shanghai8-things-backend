
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Shared.Enum;

namespace Hgzn.Mes.Application.Main.Dtos.Equip
{
    public class LocationLabelReadDto : ReadDto
    {
        public string TagId { get; set; } = null!;
        public LabelType LabelType { get; set; }

        public Guid? EquipLedgerId { get; set; }

        public EquipLedgerReadDto? EquipLedger { get; set; }

        public Guid? RoomId { get; set; }

        public RoomReadDto? Room { get; set; }

        public Guid? CreatorId { get; set; }
        public DateTime CreationTime { get; set; }
        public Guid? LastModifierId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }

    public class LocationLabelUpdateDto : UpdateDto
    {
        public string TagId { get; set; } = null!;
        public LabelType LabelType { get; set; }
        public Guid? EquipLedgerId { get; set; }
        public Guid? RoomId { get; set; }        
    }

    public class LocationLabelCreateDto : CreateDto
    {
        public string TagId { get; set; } = null!;
        public LabelType LabelType { get; set; }
        public Guid? EquipLedgerId { get; set; }
        public Guid? RoomId { get; set; }
    }

    public class LocationLabelQueryDto : PaginatedQueryDto
    {
        public LabelType LabelType { get; set; }
        public string? TagId { get; set; }

        public string? Query { get; set; }
    }


    public class BindingLabelDto
    {
        public LabelType LabelType { get; set; }

        public IEnumerable<string> Tids { get; set; } = null!;

        public Guid? EquipLedgerId { get; set; }

        public Guid? RoomId { get; set; }
    }
}
