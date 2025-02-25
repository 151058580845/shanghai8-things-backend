using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.System;

public class BuildingService : SugarCrudAppService<
        Building, Guid,
        BuildingReadDto, BuildingQueryTimeDto,
        BuildingCreateDto, BuildingUpdateDto>,
    IBuildingService
{
    public override async Task<IEnumerable<BuildingReadDto>> GetListAsync(BuildingQueryDto? queryDto = null)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto?.Name), x=> x.Name.Contains(queryDto!.Name!))
            // .Includes(t=>t.Floors)
            // .OrderBy(x=>x.OrderNum)
            .WhereIF(queryDto?.StartTime != null,t => t.CreationTime >= queryDto!.StartTime)
            .WhereIF(queryDto?.EndTime != null,t => t.CreationTime <= queryDto!.EndTime)
            .ToListAsync();
        return Mapper.Map<IEnumerable<Building>, IEnumerable<BuildingReadDto>>(entities);
    }

    public override async Task<PaginatedList<BuildingReadDto>> GetPaginatedListAsync(BuildingQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto.Name), x=> x.Name.Contains(queryDto!.Name!))
            // .Includes(t=>t.Floors)
            // .OrderBy(x=>x.OrderNum)
            .WhereIF(queryDto.StartTime != null,t => t.CreationTime >= queryDto.StartTime)
            .WhereIF(queryDto.EndTime != null,t => t.CreationTime <= queryDto.EndTime)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<Building>, PaginatedList<BuildingReadDto>>(entities);
    }
}