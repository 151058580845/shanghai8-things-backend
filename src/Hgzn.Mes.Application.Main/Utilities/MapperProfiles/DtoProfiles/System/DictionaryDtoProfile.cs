using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Dictionary;
using Hgzn.Mes.Domain.ValueObjects;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System;

public class DictionaryDtoProfile:Profile
{

    public DictionaryDtoProfile()
    {
        CreateMap<DictionaryTypeCreateDto, DictionaryType>();
        CreateMap<DictionaryType, DictionaryTypeReadDto>();
        CreateMap<ScopeDefinition, ScopeDefReadDto>();
    }
}