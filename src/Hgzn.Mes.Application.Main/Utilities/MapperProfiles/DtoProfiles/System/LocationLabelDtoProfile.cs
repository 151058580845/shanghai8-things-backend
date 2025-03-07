using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System;

public class LocationLabelDtoProfile : Profile
{
    public LocationLabelDtoProfile()
    {
        CreateMap<LocationLabel, LocationLabelReadDto>()
            .ForMember(dest => dest.LabelType, opts => opts.MapFrom(src => src.Type));
    }
    
}