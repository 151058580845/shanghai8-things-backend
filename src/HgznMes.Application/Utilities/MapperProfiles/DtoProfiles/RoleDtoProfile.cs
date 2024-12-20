using AutoMapper;
using HgznMes.Application.Dtos;
using HgznMes.Domain.Shared;
using HgznMes.Domain.Entities;
using HgznMes.Domain.Entities.Login;

namespace HgznMes.Application.Utilities.MapperProfiles.DtoProfiles
{
    public class RoleDtoProfile : Profile
    {
        public RoleDtoProfile()
        {
            CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>)).ConvertUsing(typeof(PaginatedListConverter<,>));
            CreateMap<RoleCreateDto, Role>()
                .ForMember(dest => dest.Scopes, opt => opt.Ignore());
            CreateMap<Role, RoleReadDto>();
            CreateMap<Scope, RoleScopeReadDto>();
        }
    }
}