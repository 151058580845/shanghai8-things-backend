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
            .WhereIF(queryDto != null && !string.IsNullOrEmpty(queryDto.Code), x=> x.Code.Contains(queryDto!.Code!))
            .Includes(t=>t.Rooms)
            .OrderBy(x=>x.OrderNum)
            .ToListAsync();
        return Mapper.Map<IEnumerable<Floor>, IEnumerable<FloorReadDto>>(entities);
    }

    public override async Task<PaginatedList<FloorReadDto>> GetPaginatedListAsync(FloorQueryDto queryDto)
    {
        LoggerAdapter.LogInformation($"FloorService - 搜索参数: Name={queryDto.Name ?? "null"}, Code={queryDto.Code ?? "null"}, PageIndex={queryDto.PageIndex}, PageSize={queryDto.PageSize}");
        LoggerAdapter.LogInformation($"FloorService - Code是否为空: IsNullOrEmpty={string.IsNullOrEmpty(queryDto.Code)}");
        
        var queryable = Queryable;
        
        if (!string.IsNullOrEmpty(queryDto.Name))
        {
            LoggerAdapter.LogInformation($"FloorService - 添加Name过滤条件: {queryDto.Name}");
            queryable = queryable.Where(x => x.Name!.Contains(queryDto.Name!));
        }
        
        if (!string.IsNullOrEmpty(queryDto.Code))
        {
            LoggerAdapter.LogInformation($"FloorService - 添加Code过滤条件: {queryDto.Code}");
            queryable = queryable.Where(x => x.Code!.Contains(queryDto.Code!));
        }
        
        queryable = queryable.Includes(t => t.Rooms).OrderBy(x => x.OrderNum);
        
        var entities = await queryable.ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        
        LoggerAdapter.LogInformation($"FloorService - 查询结果: 找到 {entities.Items.Count()} 条记录，总数 {entities.TotalCount}");
        var firstItem = entities.Items.FirstOrDefault();
        if (firstItem != null)
        {
            LoggerAdapter.LogInformation($"FloorService - 第一条记录: Name={firstItem.Name}, Code={firstItem.Code}");
        }
        
        return Mapper.Map<PaginatedList<Floor>, PaginatedList<FloorReadDto>>(entities);
    }
}