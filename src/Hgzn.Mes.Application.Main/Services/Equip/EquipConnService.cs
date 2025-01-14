using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipConnService : CrudAppServiceSugar<EquipConnect
    , Guid, EquipConnectQueryDto, EquipConnectReadDto, EquipConnectCreateDto, EquipConnectUpdateDto>
{
    private readonly EquipLedgerService _equipLedgerService;

    public EquipConnService(EquipLedgerService equipLedgerService)
    {
        this._equipLedgerService = equipLedgerService;
    }


    public override async Task<IEnumerable<EquipConnectReadDto>> GetListAsync(EquipConnectQueryDto queryDto)
    {
        var equips = await (await _equipLedgerService.GetEquipsListAsync(queryDto.EquipCode, queryDto.EquipName))
            .OrderBy(t => t.OrderNum)
            .ToListAsync();
        var equipIds = equips.Select(t => t.Id).ToList();
        if (equipIds.Count == 0)
        {
            return await Task.FromResult(Enumerable.Empty<EquipConnectReadDto>());
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

        foreach (var outputDto in outputs)
        {
            if (equipDictionary.TryGetValue(outputDto.EquipId, out var entity))
            {
                outputDto.EquipCode = entity.EquipCode;
                outputDto.EquipName = entity.EquipCode;
                outputDto.TypeName = entity.EquipTypeAggregate?.TypeName;
                outputDto.ConnectState = await _equipConnectManager.IsConnectedAsync(outputDto.Id);
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

        return new PagedResultDto<EquipConnectGetListOutputDto>(total, outputs);
    }

    public async Task<List<EquipConnectReadDto>> MapToGetListOutputDtosAsync(List<EquipConnect> equipLedgerQueryDtos)
    {
        var dots = Mapper.Map<List<EquipConnectReadDto>>(equipLedgerQueryDtos);
        return await Task.FromResult(dots);
    }

    /// <summary>
    /// 判断设备连接状态
    /// </summary>
    /// <param name="connectionId">设备参数配置Id</param>
    /// <returns></returns>
    public async Task<bool> IsConnectedAsync(Guid connectionId)
    {
        var entity = await CacheEquipStatus.GetOrAddAsync(connectionId.ToString(), async () => new EquipStatus()
        {
            ConnectId = connectionId,
            ConnectStatus = await EquipControlHelp.IsConnectedAsync(connectionId)
        });
        return entity.ConnectStatus;
    }
}