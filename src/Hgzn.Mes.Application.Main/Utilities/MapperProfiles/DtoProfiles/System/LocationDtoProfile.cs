using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Location;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System;

public class LocationDtoProfile:Profile
{
    public LocationDtoProfile()
    {
        CreateMap<Floor, FloorReadDto>()
            .ForMember(t => t.NumberOfRooms, o => o.MapFrom(p => p.Rooms == null?0:p.Rooms.Count));
    }
}