using AutoMapper;

using Hgzn.Mes.Application.Main.Dtos.Basic;
using Hgzn.Mes.Domain.Entities.Basic;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.Basic
{
    public class UnitDtoProfile : Profile
    {
        public UnitDtoProfile()
        {
            CreateMap<UnitCreateDto, Unit>();

            CreateMap<UnitReadDto, Unit>();
            CreateMap<Unit, UnitReadDto>();

            CreateMap<UnitReadDto, UnitUpdateDto>();
            CreateMap<UnitUpdateDto, UnitReadDto>();
            CreateMap<Unit, UnitReadDto>();
        }
    }
}
