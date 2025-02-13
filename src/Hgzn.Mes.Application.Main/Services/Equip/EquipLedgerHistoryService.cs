using Hgzn.Mes.Application.Main.Dtos.Base;
using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;
using StackExchange.Redis;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipLedgerHistoryService : SugarCrudAppService<
    EquipLedgerHistory, Guid,
    EquipLedgerHistoryReadDto, EquipLedgerHistoryQueryDto,
    EquipLedgerHistoryCreateDto>,
    IEquipLedgerHistoryService
{
    public override async Task<IEnumerable<EquipLedgerHistoryReadDto>> GetListAsync(EquipLedgerHistoryQueryDto? queryDto = null)
    {
        var entities = await Queryable
            .WhereIF(!queryDto.EquipId.IsGuidEmpty(),t=>t.EquipId == queryDto.EquipId!.Value)
            .WhereIF(!string.IsNullOrEmpty(queryDto.EquipCode), t=>t.EquipCode.Contains(queryDto.EquipCode!))
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipLedgerHistoryReadDto>>(entities);
    }

    public override async Task<PaginatedList<EquipLedgerHistoryReadDto>> GetPaginatedListAsync(EquipLedgerHistoryQueryDto queryDto)
    {
       var entities = await Queryable
           .WhereIF(queryDto.Type != 0,t=>t.Type==queryDto.Type!)
            .WhereIF(!queryDto.EquipId.IsGuidEmpty(),t=>t.EquipId == queryDto.EquipId!.Value)
            .WhereIF(!string.IsNullOrEmpty(queryDto.EquipCode), t=>t.EquipCode.Contains(queryDto.EquipCode!))
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
       return Mapper.Map<PaginatedList<EquipLedgerHistoryReadDto>>(entities);
    }

    /// <summary>
    /// 批量创建
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public async Task<int> CreateAsync(List<EquipLedgerHistoryCreateDto> list)
    {
        var entities = Mapper.Map<List<EquipLedgerHistory>>(list);
        return await DbContext.Insertable(entities).ExecuteCommandAsync();
    }
}