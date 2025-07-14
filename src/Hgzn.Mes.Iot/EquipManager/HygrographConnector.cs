using System.Text;
using System.Text.Json;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Entities.System.Location;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Iot.EquipConnectManager;
using RK.NetDevice.SDK.P2;
using RK.NetDevice.SDK.P2.Data;
using SqlSugar;
using StackExchange.Redis;

namespace Hgzn.Mes.Iot.EquipManager;

public class HygrographConnector : EquipConnectorBase
{
    private RKServer _server;
    private List<EquipLedger> _equipLedgers;
    private Dictionary<int, EquipLedger> _dictionary = new();
    private Dictionary<string, Room> _dictionaryRoom = new();
    public HygrographConnector(IConnectionMultiplexer connectionMultiplexer,
        IMqttExplorer mqttExplorer,
        ISqlSugarClient sugarClient,
        string uri,
        EquipConnType connType)
        : base(connectionMultiplexer, mqttExplorer, sugarClient, uri, connType)
    {
        _equipLedgers = sugarClient.Queryable<EquipLedger>()
            .Where(t => t.EquipType!.ProtocolEnum == EquipConnType.RKServer.ToString()).ToList();
        var rooms = sugarClient.Queryable<Room>().ToList();
        foreach (var equipLedger in _equipLedgers)
        {
            if (int.TryParse(equipLedger.EquipCode, out int code))
            {
                _dictionary.Add(code, equipLedger);
            }
        }

        foreach (var room in rooms)
        {
            _dictionaryRoom.Add(room.Id.ToString(), room);
        }
    }

    private void ServerOnOnReceiveRealtimeData(RKServer server, RealTimeData data)
    {
        var deviceId = data.DeviceID;
        var data1 = data.NodeList[0];
        var entity = new HygrographData()
        {
            EquipCode = deviceId.ToString(),
            EquipId = _dictionary[deviceId].Id,
            IpAddress = _dictionary[deviceId].IpAddress,
            RoomId = _dictionary[deviceId]?.RoomId,
            RoomName = _dictionaryRoom[_dictionary[deviceId].RoomId.ToString()!].Name,
            Temperature = data1.Tem,
            Humidness = data1.Hum,
            CreateTime = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
        };
        Console.WriteLine(JsonSerializer.Serialize(entity));

        _mqttExplorer.PublishAsync(IotTopicBuilder
            .CreateIotBuilder()
            .WithPrefix(TopicType.Iot)
            .WithDirection(MqttDirection.Up)
            .WithTag(MqttTag.Data)
            .WithUri(_uri!)
            .WithDeviceType(_connType.ToString()!)
            .Build(), Encoding.UTF8.GetBytes(JsonSerializer.Serialize(entity, DefaultJsonSerializerOptions)));
    }

    public override async Task CloseConnectionAsync()
    {
        _server.Stop();
        await UpdateStateAsync(ConnStateType.Stop);
        await UpdateOperationAsync(ConnStateType.Stop);
    }

    public override async Task StartAsync(Guid uri)
    {
        _server.Start();
        await UpdateStateAsync(ConnStateType.Run);
        await UpdateOperationAsync(ConnStateType.Run);
    }

    public override async Task StopAsync(Guid uri)
    {
        _server.Stop();
        await UpdateStateAsync(ConnStateType.Stop);
        await UpdateOperationAsync(ConnStateType.Stop);
    }

    public override async Task<bool> ConnectAsync(ConnInfo connInfo)
    {
        if (connInfo?.ConnString is null) throw new ArgumentNullException("connIfo");
        SocketConnInfo conn = JsonSerializer.Deserialize<SocketConnInfo>(connInfo.ConnString, Options.CustomJsonSerializerOptions) ?? throw new ArgumentNullException("conn");
        try
        {
            _server = RKServer.Initiate(conn.Address, conn.Port);
            _server.OnReceiveRealtimeData += ServerOnOnReceiveRealtimeData;
            _server.Start();
            await UpdateStateAsync(ConnStateType.On);
            await UpdateOperationAsync(ConnStateType.On);
            LoggerAdapter.LogInformation($"ip: {conn.Address}, port: {conn.Port}, server start sucessed!");
        }
        catch (Exception)
        {
            await CloseConnectionAsync();
            LoggerAdapter.LogInformation($"ip: {conn.Address}, port: {conn.Port}, server start failure!");
        }

        return false;
    }

    public override Task SendDataAsync(byte[] buffer)
    {
        return Task.CompletedTask;
    }
}