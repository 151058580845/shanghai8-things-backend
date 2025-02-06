using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System
{
    public class RoleDtoProfile : Profile
    {
        public RoleDtoProfile()
        {
            CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>)).ConvertUsing(typeof(PaginatedListConverter<,>));
            CreateMap<RoleCreateDto, Role>();
            CreateMap<Role, RoleReadDto>();
            CreateMap<ScopeDefinition, ScopeDefReadDto>();
        }
    }
}