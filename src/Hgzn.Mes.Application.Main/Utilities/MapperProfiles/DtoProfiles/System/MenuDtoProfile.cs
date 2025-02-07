using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Authority;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System
{
    public class MenuDtoProfile : Profile
    {
        public MenuDtoProfile()
        {
            CreateMap<Menu, MenuReadDto>()
                .ForMember(dest=>dest.Path,opt=>opt.MapFrom(src=>src.Route));
            CreateMap<MenuCreateDto, Menu>()
                .ForMember(dest=>dest.Route,opt=>opt.MapFrom(src=>src.Path));
            CreateMap<MenuUpdateDto, Menu>()
                .ForMember(dest=>dest.Route,opt=>opt.MapFrom(src=>src.Path));
        }
    }
}