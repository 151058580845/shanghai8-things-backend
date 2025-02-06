using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Location;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface IRoomService : ICrudAppService<
    Room, Guid,
    RoomReadDto, RoomQueryDto,
    RoomCreateDto, RoomUpdateDto>
{
}