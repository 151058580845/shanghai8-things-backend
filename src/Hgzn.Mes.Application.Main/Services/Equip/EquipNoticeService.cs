using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enum;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipNoticeService : SugarCrudAppService<
        EquipNotice, Guid,
        EquipNoticeReadDto, EquipNoticeQueryDto,
        EquipNoticeCreateDto>,
    IEquipNoticeService
{
    public override async Task<IEnumerable<EquipNoticeReadDto>> GetListAsync(EquipNoticeQueryDto? queryDto = null)
    {
        var entities = await Queryable
            .WhereIF(!queryDto!.EquipId.IsGuidEmpty(),t=>t.EquipId == queryDto.EquipId!.Value)
            .WhereIF(!string.IsNullOrEmpty(queryDto.Title), t=>t.Title!.Contains(queryDto.Title!))
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipNoticeReadDto>>(entities);
    }

    public override async Task<PaginatedList<EquipNoticeReadDto>> GetPaginatedListAsync(EquipNoticeQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(!queryDto.EquipId.IsGuidEmpty(),t=>t.EquipId == queryDto.EquipId!.Value)
            .WhereIF(!string.IsNullOrEmpty(queryDto.Title), t=>t.Title!.Contains(queryDto.Title!))
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        return Mapper.Map<PaginatedList<EquipNoticeReadDto>>(entities);
    }
}