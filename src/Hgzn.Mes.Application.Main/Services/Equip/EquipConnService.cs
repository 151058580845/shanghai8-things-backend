using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using SqlSugar;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipConnService : SugarCrudAppService<
    EquipConnect, Guid,
    EquipConnectReadDto, EquipConnectQueryDto,
    EquipConnectCreateDto, EquipConnectUpdateDto>,
    IEquipConnService
{
    private readonly IEquipLedgerService _equipLedgerService;
    private readonly IMqttExplorer _mqttExplorer;

    public EquipConnService(IEquipLedgerService equipLedgerService, IMqttExplorer mqttExplorer)
    {
        _equipLedgerService = equipLedgerService;
        _mqttExplorer = mqttExplorer;
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
        List<EquipConnect> entities = new List<EquipConnect>();
        try
        {
            entities = await query
               .Skip(queryDto.PageIndex)
               .Take(queryDto.PageSize)
               .Includes(t => t.EquipLedger, eq => eq.EquipTypeAggregate)
               .ToListAsync();
        }
        catch (Exception e) { }
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

    public async override Task<IEnumerable<EquipConnectReadDto>> GetListAsync(EquipConnectQueryDto? queryDto)
    {
        var equips = await (await _equipLedgerService.GetEquipsListAsync(queryDto.EquipCode, queryDto.EquipName))
            .OrderBy(t => t.OrderNum)
            .ToListAsync();
        var equipIds = equips.Select(t => t.Id).ToList();
        if (equipIds.Count == 0)
        {
            return new List<EquipConnectReadDto>();
        }

        var query = Queryable
            .Where(t => equipIds.Contains(t.EquipId));
        RefAsync<int> total = await query.CountAsync();
        var entities = await query
            .Skip(queryDto.PageIndex)
            .Take(queryDto.PageSize)
            .Includes(t => t.EquipLedger, eq => eq.EquipTypeAggregate)
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
        return new List<EquipConnectReadDto>(outputs);
    }

    public async override Task<EquipConnectReadDto?> UpdateAsync(Guid key, EquipConnectUpdateDto dto)
    {
        EquipConnectReadDto? entity = await base.UpdateAsync(key, dto);
        if (entity == null) return entity;
        EquipConnect equipConnect = await Queryable.FirstAsync(x => x.Id == key);
        if (equipConnect == null) return entity;
        await PutStartConnect(entity.Id);
        return entity;
    }

    /// <summary>
    /// 启动连接
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task PutStartConnect(Guid id)
    {
        EquipConnect connect = await Queryable.Where(it => it.Id == id)
           .Includes(t => t.EquipLedger)
           .FirstAsync();
        IotTopicBuilder iotTopicBuilder = IotTopicBuilder.CreateIotBuilder()
                .WithPrefix(TopicType.Iot)
                .WithDirection(MqttDirection.Down)
                .WithTag(MqttTag.Cmd)
                .WithDeviceType(connect.ProtocolEnum.ToString())
                .WithUri(connect.EquipId.ToString());

        ConnInfo<SocketConnInfo> info = new ConnInfo<SocketConnInfo>();
        info.ConnType = connect.ProtocolEnum;
        //info.Content = connect.ConnectStr;
        info.Type = CmdType.Conn;
        string socketDto = JsonSerializer.Serialize(info);
        byte[] msg = Encoding.UTF8.GetBytes(socketDto);
        string topic = iotTopicBuilder.Build();
        if (await _mqttExplorer.IsConnectedAsync())
        {
            await _mqttExplorer.PublishAsync(topic, msg);
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    /// <param name="connectId">配置Id</param>
    /// <returns></returns>
    public async Task StopConnectAsync(Guid connectId)
    {
        IotTopicBuilder iotTopicBuilder = IotTopicBuilder.CreateIotBuilder()
                .WithPrefix(TopicType.Iot)
                .WithDirection(MqttDirection.Down)
                .WithTag(MqttTag.Cmd)
                .WithUri(connectId.ToString());
        string topic = iotTopicBuilder.Build();
        await _mqttExplorer.PublishAsync(topic, null);
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    /// <param name="protocolEnum"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task TestConnection(Protocol protocolEnum, string connectionString)
    {
        var guid = Guid.NewGuid();
        var connect = new EquipConnect()
        {
            Id = guid,
            Name = "测试123"
        };
        connect.ProtocolEnum = protocolEnum;
        await PutStartConnect(guid);
    }
}