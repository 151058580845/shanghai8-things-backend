using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Utilities;
using Hgzn.Mes.Infrastructure.Utilities;
using System.Collections.Generic;
using System.Linq;

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
        // 如果提供了设备名称或编号，先查询符合条件的设备ID
        List<Guid>? equipIds = null;
        if (!string.IsNullOrEmpty(queryDto.EquipName) || !string.IsNullOrEmpty(queryDto.EquipCode))
        {
            var equipQuery = DbContext.Queryable<EquipLedger>()
                .Where(t => !string.IsNullOrEmpty(t.EquipName) && !string.IsNullOrEmpty(t.AssetNumber)) // 过滤没有设备名称或资产编号的记录
                .Where(t => !t.SoftDeleted); // 过滤软删除的记录
            
            if (!string.IsNullOrEmpty(queryDto.EquipName))
            {
                var equipNameLower = queryDto.EquipName!.ToLower();
                equipQuery = equipQuery.Where(t => t.EquipName != null && t.EquipName.ToLower().Contains(equipNameLower));
            }
            if (!string.IsNullOrEmpty(queryDto.EquipCode))
            {
                var equipCodeLower = queryDto.EquipCode!.ToLower();
                equipQuery = equipQuery.Where(t => t.AssetNumber != null && t.AssetNumber.ToLower().Contains(equipCodeLower));
            }
            equipIds = await equipQuery.Select(t => t.Id).ToListAsync();
            
            LoggerAdapter.LogInformation($"EquipNoticeService - 按设备名称/编号搜索: EquipName={queryDto.EquipName}, EquipCode={queryDto.EquipCode}, 找到设备数量={equipIds?.Count ?? 0}");
            
            // 如果没有找到符合条件的设备，返回空结果
            if (equipIds == null || !equipIds.Any())
            {
                LoggerAdapter.LogInformation($"EquipNoticeService - 未找到符合条件的设备，返回空结果");
                return new PaginatedList<EquipNoticeReadDto>(
                    new List<EquipNoticeReadDto>(),
                    0,
                    queryDto.PageIndex,
                    queryDto.PageSize
                );
            }
        }

        var queryable = Queryable
            .WhereIF(!queryDto.EquipId.IsNullableGuidEmpty(), t => t.EquipId == queryDto.EquipId!.Value)
            .WhereIF(equipIds != null && equipIds.Any(), t => equipIds!.Contains(t.EquipId))
            .WhereIF(!string.IsNullOrEmpty(queryDto.Title), t => t.Title!.Contains(queryDto.Title!))
            .OrderByDescending(t => t.SendTime);
        
        var entities = await queryable.ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);
        
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