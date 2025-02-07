using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.System;

public class FloorService : SugarCrudAppService<
        Floor, Guid,
        FloorReadDto, FloorQueryDto,
        FloorCreateDto, FloorUpdateDto>,
    IFloorService
{
    public override async Task<IEnumerable<FloorReadDto>> GetListAsync(FloorQueryDto? queryDto = null)
    {
        var entities = await Queryable
            .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.Name), x=> x.Name.Contains(queryDto!.Name!))      
            // .Includes(t=>t.Floors)
            .OrderBy(x=>x.OrderNum)
            .ToListAsync();
        return Mapper.Map<IEnumerable<Floor>, IEnumerable<FloorReadDto>>(entities);
    }

    public override async Task<PaginatedList<FloorReadDto>> GetPaginatedListAsync(FloorQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto.Name), x=> x.Name!.Contains(queryDto.Name!))
            // .Includes(t=>t.Floors)
            .OrderBy(x=>x.OrderNum)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<Floor>, PaginatedList<FloorReadDto>>(entities);
    }
}