using Hgzn.Mes.Application.Main.Dtos.System;
using Hgzn.Mes.Application.Main.Services.System.IService;
using Hgzn.Mes.Domain.Entities.System;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.System;

public class DepartmentService : SugarCrudAppService<
        Dept, Guid,
        DepartmentReadDto, DepartmentQueryDto,
        DepartmentCreateDto, DepartmentUpdateDto>,
    IDepartmentService
{
    public override async Task<IEnumerable<DepartmentReadDto>> GetListAsync(DepartmentQueryDto? input = null)
    {
        var entities = await Queryable
            // .WhereIF(!string.IsNullOrEmpty(input?.DeptName), u => input != null && u.DeptName.Contains(input.DeptName!))
            // .WhereIF(input is { State: not null }, u => input != null && u.State == input.State)
            .OrderBy(u => u.OrderNum, OrderByType.Asc)
            .ToListAsync();
        return Mapper.Map<IEnumerable<DepartmentReadDto>>(entities);
    }

    public override async Task<PaginatedList<DepartmentReadDto>> GetPaginatedListAsync(DepartmentQueryDto input)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(input.DeptName), u => u.DeptName.Contains(input.DeptName!))
            .WhereIF(input.State is not null, u => u.State == input.State)
            .OrderBy(u => u.OrderNum, OrderByType.Asc)
            .ToPaginatedListAsync(input.PageIndex, input.PageSize);
        return Mapper.Map<PaginatedList<DepartmentReadDto>>(entities);
    }

    public async Task<List<Guid>> GetChildListAsync(Guid deptId)
    {
        var entities = await Queryable.ToChildListAsync(x => x.ParentId, deptId);
        return await entities.Select(x => x.Id).ToListAsync();
    }
}