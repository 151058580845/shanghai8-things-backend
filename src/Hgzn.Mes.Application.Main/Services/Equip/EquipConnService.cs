using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.ProtocolManagers;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Infrastructure.DomainServices;
using Microsoft.Extensions.Caching.Memory;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipConnService : CrudAppServiceSugar<EquipConnect
    , Guid, EquipConnectQueryDto, EquipConnectReadDto, EquipConnectCreateDto, EquipConnectUpdateDto>
{
    private readonly EquipLedgerService _equipLedgerService;
    private readonly IMemoryCacheDomainService _memoryCacheDomainService;

    public EquipConnService(EquipLedgerService equipLedgerService, IMemoryCacheDomainService memoryCacheDomainService)
    {
        this._equipLedgerService = equipLedgerService;
        this._memoryCacheDomainService = memoryCacheDomainService;
    }


    public override async Task<PaginatedList<EquipConnectReadDto>> GetListAsync(EquipConnectQueryDto queryDto)
    {
        var equips = await (await _equipLedgerService.GetEquipsListAsync(queryDto.EquipCode, queryDto.EquipName))
            .OrderBy(t => t.OrderNum)
            .ToListAsync();
        var equipIds = equips.Select(t => t.Id).ToList();
        if (equipIds.Count == 0)
        {
            return new PaginatedList<EquipConnectReadDto>((Enumerable.Empty<EquipConnectReadDto>()), 0, queryDto.PageIndex, queryDto.PageSize);
        }

        var query = Queryable()
            .Where(t => equipIds.Contains(t.EquipId));
        RefAsync<int> total = await query.CountAsync();
        List<EquipConnect> entities = await query
            .Skip(queryDto.PageIndex)
            .Take(queryDto.PageSize)
            .Includes(t => t.ForwardEntities)
            .Includes(t => t.EquipLedger, eq => eq.EquipTypeAggregate)
            .ToListAsync();
        List<EquipConnectReadDto> outputs = await MapToGetListOutputDtosAsync(entities);
        var equipDictionary = entities
            .Select(t => t.EquipLedger)
            .GroupBy(t => t.Id)
            .ToDictionary(g => g.Key, g => g.First()); // 获取每组的第一个实体;

        foreach (EquipConnectReadDto outputDto in outputs)
        {
            if (equipDictionary.TryGetValue(outputDto.EquipId, out var entity))
            {
                outputDto.EquipCode = entity.EquipCode;
                outputDto.EquipName = entity.EquipCode;
                outputDto.TypeName = entity.EquipTypeAggregate?.TypeName;
                outputDto.ConnectState = await IsConnectedAsync(outputDto.EquipId);
                outputDto.ConnectStateStr = outputDto.ConnectState ? "已连接" : "未连接";
                // 判断是否为 RFID 设备，并填充状态
                if (outputDto.ProtocolEnum == ProtocolEnum.RfidReaderClient)
                {
                    outputDto.CollectionModel = outputDto.CollectionExtension switch
                    {
                        1 => "绑定Rfid标签",
                        2 => "解绑Rfid标签",
                        _ => "采集数据"
                    };
                }
            }
        }
        return new PaginatedList<EquipConnectReadDto>(outputs, total, queryDto.PageIndex, queryDto.PageSize);
    }

    public async Task<List<EquipConnectReadDto>> MapToGetListOutputDtosAsync(List<EquipConnect> equipLedgerQueryDtos)
    {
        var dots = Mapper.Map<List<EquipConnectReadDto>>(equipLedgerQueryDtos);
        return await Task.FromResult(dots);
    }

    public async Task<bool> IsConnectedAsync(Guid connectionId)
    {
        var entity = await _memoryCacheDomainService.GetOrAddAsync(
            connectionId.ToString(),
            async () => new EquipStatus
            {
                ConnectId = connectionId,
                ConnectStatus = await EquipControlHelp.IsConnectedAsync(connectionId)
            });

        return entity.ConnectStatus;
    }
}