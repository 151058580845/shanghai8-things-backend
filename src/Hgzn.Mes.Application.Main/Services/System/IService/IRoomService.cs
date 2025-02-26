using System.Collections;
using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface IRoomService : ICrudAppService<
    Room, Guid,
    RoomReadDto, RoomQueryDto,
    RoomCreateDto, RoomUpdateDto>
{
    Task<IEnumerable<NameValueListDto>>  GetRoomListAllAsync(RoomQueryDto? input);
    Task<IEnumerable<RoomReadDto>> GetRoomListByTestName(string testName);
}