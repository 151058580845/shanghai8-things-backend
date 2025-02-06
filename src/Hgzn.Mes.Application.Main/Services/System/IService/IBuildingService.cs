using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Domain.Entities.System.Dictionary;
using Hgzn.Mes.Domain.Entities.System.Location;

namespace Hgzn.Mes.Application.Main.Services.System.IService;

public interface IBuildingService : ICrudAppService<
    Building, Guid,
    BuildingReadDto, BuildingQueryDto,
    BuildingCreateDto, BuildingUpdateDto>
{
}