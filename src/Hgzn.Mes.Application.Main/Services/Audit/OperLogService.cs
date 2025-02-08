using Hgzn.Mes.Application.Main.Dtos.Audit;
using Hgzn.Mes.Application.Main.Services.Audit.IService;
using Hgzn.Mes.Domain.Entities.Audit;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.Audit;

public class OperLogService : SugarCrudAppService<
        OperatorLog, Guid,
        OperatorLogReadDto, OperatorLogQueryDto>,
    IOperLogService
{
    public override async Task<IEnumerable<OperatorLogReadDto>> GetListAsync(OperatorLogQueryDto? queryDto = null)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto?.Title), x => x.Title.Contains(queryDto.Title!))
            .ToListAsync();
        return Mapper.Map<IEnumerable<OperatorLogReadDto>>(entities);
    }

    public override async Task<PaginatedList<OperatorLogReadDto>> GetPaginatedListAsync(OperatorLogQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto.Title), x => x.Title.Contains(queryDto.Title!))
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<OperatorLogReadDto>>(entities);
    }
}