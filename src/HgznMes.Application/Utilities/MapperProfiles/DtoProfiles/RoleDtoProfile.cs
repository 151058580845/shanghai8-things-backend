using AutoMapper;
using HgznMes.Application.Dtos;
using HgznMes.Domain.Shared;
using HgznMes.Domain.ValueObjects;
using HgznMes.Domain.Entities.System.Account;

namespace HgznMes.Application.Utilities.MapperProfiles.DtoProfiles
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