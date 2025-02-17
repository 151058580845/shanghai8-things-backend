using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Services.Audit.IService;
using Hgzn.Mes.Domain.Entities.Audit;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.Audit;

public class OperatorLogService : SugarCrudAppService<
        OperatorLog, Guid,
        OperatorLogReadDto, OperatorLogQueryDto>,
    IOperLogService
{
    public override async Task<IEnumerable<OperatorLogReadDto>> GetListAsync(OperatorLogQueryDto? queryDto = null)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto?.Title), x => x.Title.Contains(queryDto!.Title!))
            .WhereIF(queryDto?.StartTime != null,t => t.CreationTime >= queryDto!.StartTime)
            .WhereIF(queryDto?.EndTime != null,t => t.CreationTime <= queryDto!.EndTime)
            .ToListAsync();
        return Mapper.Map<IEnumerable<OperatorLogReadDto>>(entities);
    }

    public override async Task<PaginatedList<OperatorLogReadDto>> GetPaginatedListAsync(OperatorLogQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto.Title), x => x.Title.Contains(queryDto.Title!))
            .WhereIF(queryDto.StartTime != null,t => t.CreationTime >= queryDto.StartTime)
            .WhereIF(queryDto.EndTime != null,t => t.CreationTime <= queryDto.EndTime)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<OperatorLogReadDto>>(entities);
    }

    /// <summary>
    /// 删除全部日志
    /// </summary>
    /// <returns></returns>
    public async Task<int> DeleteAllLoginfo()
    {

        var entities = await Queryable.Select(a => a.Id).ToListAsync();
        var delcount = 0;
        if (entities.Any())
        {
            delcount = await DbContext.Deleteable<OperatorLog>().Where(s => entities.Contains(s.Id)).ExecuteCommandAsync();
        }

        return delcount;
    }
}