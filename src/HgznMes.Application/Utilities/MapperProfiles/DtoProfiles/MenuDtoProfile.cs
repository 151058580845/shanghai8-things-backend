using AutoMapper;
using HgznMes.Application.Dtos;
using HgznMes.Domain.Entities.Authority;

namespace HgznMes.Application.Utilities.MapperProfiles.DtoProfiles
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
