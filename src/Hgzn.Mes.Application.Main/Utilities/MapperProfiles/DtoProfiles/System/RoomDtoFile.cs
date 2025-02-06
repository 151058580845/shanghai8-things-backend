using AutoMapper;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Location;

namespace Hgzn.Mes.Application.Main.Utilities.MapperProfiles.DtoProfiles.System;

public class RoomDtoFile:Profile
{
    public RoomDtoFile()
    {
        CreateMap<RoomReadDto, RoomQrCode>();
    }
    
}