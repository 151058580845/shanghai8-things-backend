using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Code;
using Hgzn.Mes.Domain.ValueObjects;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System;

public class CodeDtoProfile:Profile
{

    public CodeDtoProfile()
    {
        CreateMap<CodeRuleCreateDto, CodeRule>();
        CreateMap<CodeRule, CodeRuleReadDto>();
        CreateMap<ScopeDefinition, ScopeDefReadDto>();
    }
}