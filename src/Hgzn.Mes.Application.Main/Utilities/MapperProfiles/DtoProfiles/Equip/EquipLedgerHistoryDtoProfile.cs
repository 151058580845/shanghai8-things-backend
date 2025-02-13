using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Equip;

public class EquipLedgerHistoryDtoProfile : Profile
{
    public EquipLedgerHistoryDtoProfile()
    {
        CreateMap<EquipLedgerHistoryCreateDto, EquipLedgerHistory>()
            .ForMember(d => d.OperatorTime, opt => opt.MapFrom(x =>new DateTime(x.OperatorTime)));

    }
}