using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Domain.Entities.System.Authority;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles
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
