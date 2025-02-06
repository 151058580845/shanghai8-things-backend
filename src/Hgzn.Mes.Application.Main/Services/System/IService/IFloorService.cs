using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Location;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface IFloorService : ICrudAppService<
    Floor, Guid,
    FloorReadDto, FloorQueryDto,
    FloorCreateDto, FloorUpdateDto>
{
}