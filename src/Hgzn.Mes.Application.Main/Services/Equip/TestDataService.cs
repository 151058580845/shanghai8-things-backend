using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class TestDataService : SugarCrudAppService<
        TestData, Guid,
        TestDataReadDto, TestDataQueryDto,
        TestDataCreateDto, TestDataUpdateDto>,
    ITestDataService
{
    public override async Task<IEnumerable<TestDataReadDto>> GetListAsync(TestDataQueryDto? queryDto = null)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto.SysName),t=>t.SysName.Contains(queryDto.SysName))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ProjectName),t=>t.SysName.Contains(queryDto.ProjectName))
            .WhereIF(!string.IsNullOrEmpty(queryDto.TaskName),t=>t.SysName.Contains(queryDto.TaskName))
            .WhereIF(!string.IsNullOrEmpty(queryDto.DevPhase),t=>t.SysName.Contains(queryDto.DevPhase))
            .WhereIF(!string.IsNullOrEmpty(queryDto.TaskStartTime),t=>t.SysName.Contains(queryDto.TaskStartTime))
            .WhereIF(!string.IsNullOrEmpty(queryDto.TaskEndTime),t=>t.SysName.Contains(queryDto.TaskEndTime))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ReqDep),t=>t.SysName.Contains(queryDto.ReqDep))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ReqManager),t=>t.SysName.Contains(queryDto.ReqManager))
            .WhereIF(!string.IsNullOrEmpty(queryDto.SimuResp),t=>t.SysName.Contains(queryDto.SimuResp))
            .WhereIF(!string.IsNullOrEmpty(queryDto.SimuStaff),t=>t.SysName.Contains(queryDto.SimuStaff))
            .WhereIF(!string.IsNullOrEmpty(queryDto.QncResp),t=>t.SysName.Contains(queryDto.QncResp))
            .ToListAsync();
        return Mapper.Map<IEnumerable<TestDataReadDto>>(entities);
    }

    public override async Task<PaginatedList<TestDataReadDto>> GetPaginatedListAsync(TestDataQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(!string.IsNullOrEmpty(queryDto.SysName),t=>t.SysName.Contains(queryDto.SysName))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ProjectName),t=>t.SysName.Contains(queryDto.ProjectName))
            .WhereIF(!string.IsNullOrEmpty(queryDto.TaskName),t=>t.SysName.Contains(queryDto.TaskName))
            .WhereIF(!string.IsNullOrEmpty(queryDto.DevPhase),t=>t.SysName.Contains(queryDto.DevPhase))
            .WhereIF(!string.IsNullOrEmpty(queryDto.TaskStartTime),t=>t.SysName.Contains(queryDto.TaskStartTime))
            .WhereIF(!string.IsNullOrEmpty(queryDto.TaskEndTime),t=>t.SysName.Contains(queryDto.TaskEndTime))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ReqDep),t=>t.SysName.Contains(queryDto.ReqDep))
            .WhereIF(!string.IsNullOrEmpty(queryDto.ReqManager),t=>t.SysName.Contains(queryDto.ReqManager))
            .WhereIF(!string.IsNullOrEmpty(queryDto.SimuResp),t=>t.SysName.Contains(queryDto.SimuResp))
            .WhereIF(!string.IsNullOrEmpty(queryDto.SimuStaff),t=>t.SysName.Contains(queryDto.SimuStaff))
            .WhereIF(!string.IsNullOrEmpty(queryDto.QncResp),t=>t.SysName.Contains(queryDto.QncResp))
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<TestDataReadDto>>(entities);
    }
}