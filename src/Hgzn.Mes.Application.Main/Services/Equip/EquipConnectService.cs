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
using System.Text;
using System.Text.Json;

namespace Hgzn.Mes.Application.Main.Services.Equip;

public class EquipConnectService : SugarCrudAppService<
    EquipConnect, Guid,
    EquipConnectReadDto, EquipConnectQueryDto,
    EquipConnectCreateDto, EquipConnectUpdateDto>,
    IEquipConnService
{
    private readonly IEquipLedgerService _equipLedgerService;
    private readonly IMqttExplorer _mqttExplorer;

    public EquipConnectService(IEquipLedgerService equipLedgerService, IMqttExplorer mqttExplorer)
    {
        _equipLedgerService = equipLedgerService;
        _mqttExplorer = mqttExplorer;
    }


    public override async Task<PaginatedList<EquipConnectReadDto>> GetPaginatedListAsync(EquipConnectQueryDto queryDto)
    {
        var querable = await DbContext.Queryable<EquipConnect>()
            .Includes(eq => eq.EquipLedger, el => el.EquipType)
            .WhereIF(!queryDto.EquipName.IsNullOrEmpty(), eq => eq.EquipLedger!.EquipName.Contains(queryDto.EquipName!))
            .WhereIF(!queryDto.EquipCode.IsNullOrEmpty(), eq => eq.EquipLedger!.EquipCode.Contains(queryDto.EquipCode!))
            .OrderBy(t => t.OrderNum)
            .ToPaginatedListAsync(queryDto.PageIndex, queryDto.PageSize);

        return Mapper.Map<PaginatedList<EquipConnectReadDto>>(querable);
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
            .Includes(eq => eq.EquipLedger, el => el.EquipType)
            .OrderBy(t => t.OrderNum)
            .ToArrayAsync();
        return Mapper.Map<IEnumerable<EquipConnectReadDto>>(result);
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
           .Includes(t => t.EquipLedger, le => le.EquipType)
           .FirstAsync();

        var conninfo = new ConnInfo
        {
            ConnType = connect.ProtocolEnum,
            ConnString = connect.ConnectStr,
            Type = CmdType.Conn,
            StateType = ConnStateType.On,
        };
        var startInfo = new ConnInfo
        {
            ConnType = connect.ProtocolEnum,
            ConnString = connect.ConnectStr,
            Type = CmdType.Conn,
            StateType = ConnStateType.Run,
        };

        var topic = IotTopicBuilder.CreateIotBuilder()
                .WithPrefix(TopicType.Iot)
                .WithDirection(MqttDirection.Down)
                .WithTag(MqttTag.Cmd)
                .WithDeviceType(connect.EquipLedger?.EquipType?.TypeCode ??
                    throw new ArgumentNullException("equip type not exist"))
                .WithUri(connect.Id.ToString()).Build();
        if (await _mqttExplorer.IsConnectedAsync())
        {
            await _mqttExplorer.PublishAsync(topic, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(startInfo)));
            await Task.Delay(5 * 100);
            await _mqttExplorer.PublishAsync(topic, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(conninfo)));
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
           .Includes(t => t.EquipLedger, le => le.EquipType)
           .FirstAsync();

        IotTopicBuilder iotTopicBuilder = IotTopicBuilder.CreateIotBuilder()
                .WithPrefix(TopicType.Iot)
                .WithDirection(MqttDirection.Down)
                .WithTag(MqttTag.Cmd)
                .WithDeviceType(connect.EquipLedger?.EquipType?.TypeCode ??
                    throw new ArgumentNullException("equip type not exist"))
                .WithUri(connect.Id.ToString());

        string topic = iotTopicBuilder.Build();

        var stopInfo = new ConnInfo
        {
            ConnType = connect.ProtocolEnum,
            ConnString = connect.ConnectStr,
            Type = CmdType.Conn,
            StateType = ConnStateType.Off,
        };

        await _mqttExplorer.PublishAsync(topic, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(stopInfo)));
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
}