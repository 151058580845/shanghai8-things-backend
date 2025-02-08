using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipConnService : SugarCrudAppService<
    EquipConnect, Guid,
    EquipConnectReadDto, EquipConnectQueryDto,
    EquipConnectCreateDto, EquipConnectUpdateDto>,
    IEquipConnService
{
    private readonly EquipLedgerService _equipLedgerService;

    public EquipConnService(EquipLedgerService equipLedgerService)
    {
        _equipLedgerService = equipLedgerService;
    }


    public override async Task<PaginatedList<EquipConnectReadDto>> GetPaginatedListAsync(EquipConnectQueryDto queryDto)
    {
        var equips = await (await _equipLedgerService.GetEquipsListAsync(queryDto.EquipCode, queryDto.EquipName))
            .OrderBy(t => t.OrderNum)
            .ToListAsync();
        var equipIds = equips.Select(t => t.Id).ToList();
        if (equipIds.Count == 0)
        {
            return new PaginatedList<EquipConnectReadDto>((Enumerable.Empty<EquipConnectReadDto>()), 0, queryDto.PageIndex, queryDto.PageSize);
        }

        var query = Queryable
            .Where(t => equipIds.Contains(t.EquipId));
        RefAsync<int> total = await query.CountAsync();
        var entities = await query
            .Skip(queryDto.PageIndex)
            .Take(queryDto.PageSize)
            .Includes(t => t.ForwardEntities)
            .Includes(t => t.EquipLedger, eq => eq!.EquipTypeAggregate)
            .ToListAsync();
        var outputs = await MapToGetListOutputDtosAsync(entities);
        var equipDictionary = entities
            .Select(t => t.EquipLedger)
            .GroupBy(t => t!.Id)
            .ToDictionary(g => g.Key, g => g.First()); // 获取每组的第一个实体;

        foreach (EquipConnectReadDto outputDto in outputs)
        {
            if (equipDictionary.TryGetValue(outputDto.EquipId, out var entity))
            {
                outputDto.EquipCode = entity?.EquipCode;
                outputDto.EquipName = entity?.EquipCode;
                outputDto.TypeName = entity?.EquipTypeAggregate?.TypeName;
                // 判断是否为 RFID 设备，并填充状态
                if (outputDto.ProtocolEnum == Protocol.RfidReaderClient)
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

    public override Task<IEnumerable<EquipConnectReadDto>> GetListAsync(EquipConnectQueryDto? queryDto)
    {
        throw new NotImplementedException();
    }
}