using AutoMapper;
using Hgzn.Mes.Application.Dtos;
using Hgzn.Mes.Domain.Entities.System.Authority;

namespace Hgzn.Mes.Application.Utilities.MapperProfiles.DtoProfiles
{
    public class MenuDtoProfile : Profile
    {
        public MenuDtoProfile()
        {
            CreateMap<Menu, MenuReadDto>();
            CreateMap<MenuCreateDto, Menu>();
        }
    }
}
