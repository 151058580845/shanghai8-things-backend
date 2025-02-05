using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip;

public class EquipLedgerDtoProfile:Profile
{
    public EquipLedgerDtoProfile()
    {
        CreateMap<EquipLedgerCreateDto, EquipLedger>();
        CreateMap<EquipLedgerUpdateDto, EquipLedger>();
        CreateMap<EquipLedger, EquipLedgerReadDto>();
    }
}