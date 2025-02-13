using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Location;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface IRoomService : ICrudAppService<
    Room, Guid,
    RoomReadDto, RoomQueryDto,
    RoomCreateDto, RoomUpdateDto>
{
    Task<IEnumerable<NameValueListDto>>  GetRoomListAllAsync(RoomQueryDto? input);
}