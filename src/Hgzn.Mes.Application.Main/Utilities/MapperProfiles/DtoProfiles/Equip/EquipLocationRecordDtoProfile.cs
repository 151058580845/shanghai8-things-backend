using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip;

public class EquipLocationRecordDtoProfile : Profile
{
    public EquipLocationRecordDtoProfile()
    {
        CreateMap<EquipLocationRecord, EquipLocationRecordReadDto>();
    }
}
