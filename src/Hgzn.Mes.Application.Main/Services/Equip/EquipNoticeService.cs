using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
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
            .WhereIF(!queryDto!.EquipId.IsNullableGuidEmpty(),t=>t.EquipId == queryDto.EquipId!.Value)
            .WhereIF(!string.IsNullOrEmpty(queryDto.Title), t=>t.Title!.Contains(queryDto.Title!))
            .ToListAsync();
        return Mapper.Map<IEnumerable<EquipNoticeReadDto>>(entities);
    }

    public override async Task<PaginatedList<EquipNoticeReadDto>> GetPaginatedListAsync(EquipNoticeQueryDto queryDto)
    {
        var entities = await Queryable
            .WhereIF(!queryDto.EquipId.IsNullableGuidEmpty(), t => t.EquipId == queryDto.EquipId!.Value)
            .WhereIF(!string.IsNullOrEmpty(queryDto.Title), t => t.Title!.Contains(queryDto.Title!)).OrderByDescending(t => t.SendTime)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        var result = Mapper.Map<PaginatedList<EquipNoticeReadDto>>(entities);

        var ids = result.Items.Select(t => t.EquipId).Distinct();
        var equips = DbContext.Queryable<EquipLedger>().Where(t => ids.Contains(t.Id)).LeftJoin<Room>((ledger, room) => ledger.RoomId == room.Id).Select((ledger, room) => new { Id = ledger.Id, Name = ledger.EquipName, Code = ledger.AssetNumber, RoomName = room.Name }).ToList();
        foreach (var r in result.Items)
        {
            var equip = equips.FirstOrDefault(t => t.Id == r.EquipId);
            r.EquipName = equip?.Name;
            r.EquipCode = equip?.Code;
            r.RoomName = equip?.RoomName;
        }
        return result;
    }
}