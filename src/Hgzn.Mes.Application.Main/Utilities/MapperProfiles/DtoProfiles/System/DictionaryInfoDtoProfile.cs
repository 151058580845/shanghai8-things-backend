using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Dictionary;
using Hgzn.Mes.Domain.ValueObjects;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System;

public class DictionaryInfoDtoProfile:Profile
{
    public DictionaryInfoDtoProfile()
    {
        CreateMap<DictionaryInfoCreateDto, DictionaryInfo>();
        CreateMap<DictionaryInfoUpdateDto, DictionaryInfo>();
        CreateMap<DictionaryInfo, DictionaryInfoReadDto>();
        CreateMap<ScopeDefinition, ScopeDefReadDto>();
    }
}