using Hgzn.Mes.Application.Main.Dtos.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Application.Main.Utilities;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared.Extensions;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Text;
using System.Text.Json;
using Hgzn.Mes.Application.Main.Dtos.Base;
using System.Security.Policy;
using NPOI.SS.Formula.Functions;
using MySqlX.XDevAPI.Common;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipConnectService : SugarCrudAppService<
    EquipConnect, Guid,
    EquipConnectReadDto, EquipConnectQueryDto,
    EquipConnectCreateDto, EquipConnectUpdateDto>,
    IEquipConnService
{
    private readonly IEquipLedgerService _equipLedgerService;
    private readonly IMqttExplorer _mqttExplorer;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public EquipConnectService(
        IEquipLedgerService equipLedgerService,
        IMqttExplorer mqttExplorer,
        IConnectionMultiplexer connectionMultiplexer)
    {
        _equipLedgerService = equipLedgerService;
        _mqttExplorer = mqttExplorer;
        _connectionMultiplexer = connectionMultiplexer;
    }


    public override async Task<PaginatedList<EquipConnectReadDto>> GetPaginatedListAsync(EquipConnectQueryDto queryDto)
    {
        var database = _connectionMultiplexer.GetDatabase();

        var equips = await DbContext.Queryable<EquipConnect>()
            .Includes(eq => eq.EquipLedger)
            .WhereIF(!queryDto.EquipName.IsNullOrEmpty(), eq => eq.EquipLedger!.EquipName.Contains(queryDto.EquipName!))
            .WhereIF(!queryDto.EquipCode.IsNullOrEmpty(), eq => eq.EquipLedger!.EquipCode.Contains(queryDto.EquipCode!))
            .OrderBy(t => t.OrderNum)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);

        // 从redis里查出来赋值给ReadDto
        foreach (var item in equips.Items)
        {
            var key = string.Format(CacheKeyFormatter.EquipState, item.EquipLedger?.Id, item.Id);
            item.ConnectState = database.StringGet(key).TryParse(out int index) &&
                (index == (int)ConnStateType.Run || index == (int)ConnStateType.On);
        }

        return Mapper.Map<PaginatedList<EquipConnectReadDto>>(equips);
    }

    public async Task<List<EquipConnectReadDto>> MapToGetListOutputDtosAsync(List<EquipConnect> equipLedgerQueryDtos)
    {
        var dots = Mapper.Map<List<EquipConnectReadDto>>(equipLedgerQueryDtos);
        return await Task.FromResult(dots);
    }

    public async override Task<IEnumerable<EquipConnectReadDto>> GetListAsync(EquipConnectQueryDto? queryDto)
    {
        var querable = queryDto is null ? DbContext.Queryable<EquipConnect>() : DbContext.Queryable<EquipConnect>()
            .WhereIF(!queryDto.EquipName.IsNullOrEmpty(), eq => eq.EquipLedger!.EquipName.Contains(queryDto.EquipName!))
            .WhereIF(!queryDto.EquipCode.IsNullOrEmpty(), eq => eq.EquipLedger!.EquipCode.Contains(queryDto.EquipCode!));

        var result = await querable
            .Includes(eq => eq.EquipLedger, el => el!.EquipType)
            .OrderBy(t => t.OrderNum)
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<EquipConnectReadDto>>(result);
    }


    public async Task<IEnumerable<EquipConnectReadDto>> GetRfidIssuerConnectionsAsync()
    {
        var conns = await Queryable
            .Includes(ec => ec.EquipLedger)
            .Where(ec => ec.EquipLedger!.TypeId == EquipType.RfidIssuerType.Id)
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<EquipConnectReadDto>>(conns);
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
        var connect = await Queryable.Where(it => it.Id == id)
           .Includes(t => t.EquipLedger, le => le!.EquipType)
           .FirstAsync();

        switch (connect.ProtocolEnum)
        {
            case ConnType.ModbusTcp:
                await Publish(connect, CmdType.Conn, ConnStateType.On, TopicType.Iot, MqttDirection.Down, MqttTag.Cmd);
                await DbContext.Updateable<EquipConnect>()
                    .Where(ec => ec.Id == id)
                    .SetColumns(t => new EquipConnect { State = true })
                    .ExecuteCommandAsync();
                break;
            default:
                // TcpServer
                await Publish(connect, CmdType.Conn, ConnStateType.On, TopicType.Iot, MqttDirection.Down, MqttTag.Cmd);
                await Task.Delay(5 * 100);
                await Publish(connect, CmdType.Conn, ConnStateType.Run, TopicType.Iot, MqttDirection.Down, MqttTag.Cmd);
                await DbContext.Updateable<EquipConnect>()
                    .Where(ec => ec.Id == id)
                    .SetColumns(t => new EquipConnect { State = true })
                    .ExecuteCommandAsync();
                break;
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    /// <param name="connectId">配置Id</param>
    /// <returns></returns>
    public async Task StopConnectAsync(Guid connectId)
    {
        var connect = await Queryable.Where(it => it.Id == connectId)
           .Includes(t => t.EquipLedger, le => le!.EquipType)
           .FirstAsync();
        await DbContext.Updateable<EquipConnect>()
            .Where(ec => ec.Id == connectId)
            .SetColumns(t => new EquipConnect { State = false })
            .ExecuteCommandAsync();

        await Publish(connect, CmdType.Conn, ConnStateType.Off, TopicType.Iot, MqttDirection.Down, MqttTag.Cmd);
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    /// <param name="protocolEnum"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task TestConnection(ConnType protocolEnum, string connectionString)
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

    public async Task<IEnumerable<NameValueDto>> GetNameValueListAsync()
    {
        var result = await Queryable
            .OrderBy(t => t.OrderNum)
            .Select<NameValueDto>(t => new NameValueDto()
            {
                Id = t.Id,
                Name = t.Name,
                Value = t.Id.ToString()
            })
            .ToListAsync();
        return result;
    }

    private async Task Publish(EquipConnect connect, CmdType cmdType, ConnStateType connStateType, TopicType iotType, MqttDirection mqttDirection, MqttTag mqttTag)
    {
        var conninfo = new ConnInfo
        {
            ConnType = connect.ProtocolEnum,
            ConnString = connect.ConnectStr,
            Type = cmdType,
            StateType = connStateType,
        };
        var topic = IotTopicBuilder.CreateIotBuilder()
                .WithPrefix(iotType)
                .WithDirection(mqttDirection)
                .WithTag(mqttTag)
                .WithDeviceType(connect.EquipLedger?.EquipType?.ProtocolEnum ??
                    throw new ArgumentNullException("equip type not exist"))
                .WithUri(connect.Id.ToString()).Build();
        if (await _mqttExplorer.IsConnectedAsync())
        {
            await _mqttExplorer.PublishAsync(topic, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(conninfo)));
        }
    }
}