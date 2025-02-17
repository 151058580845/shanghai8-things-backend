using System.Text.Json;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Iot.EquipManager;
using SqlSugar;
using StackExchange.Redis;

namespace Hgzn.Mes.Iot.TcpServer;

public class TcpServerConnector : EquipConnectorBase
{
    private EquipTcpServer _server = null!;
    private EquipConnect _connect;


    public TcpServerConnector(IConnectionMultiplexer connectionMultiplexer,
        IMqttExplorer mqttExplorer, ISqlSugarClient sqlSugarClient, string uri, EquipConnType connType) : base(connectionMultiplexer, mqttExplorer, sqlSugarClient)
    {
        _equipConnect = _sqlSugarClient.Queryable<EquipConnect>().First(x => x.Id == Guid.Parse(uri));

        _uri = uri;
        _connType = connType;
    }

    public override async Task CloseConnectionAsync()
    {
        _server.Stop();
        await UpdateStateAsync(ConnStateType.Stop);
        await UpdateOperationAsync(ConnStateType.Stop);
    }

    public override async Task StartAsync()
    {
        _server.Start();
        await UpdateStateAsync(ConnStateType.Run);
        await UpdateOperationAsync(ConnStateType.Run);
    }

    public override async Task StopAsync()
    {
        _server.Stop();
        await UpdateStateAsync(ConnStateType.Stop);
        await UpdateOperationAsync(ConnStateType.Stop);
    }

    public override async Task<bool> ConnectAsync(ConnInfo connInfo)
    {
        if (connInfo?.ConnString is null) throw new ArgumentNullException("connIfo");
        switch (connInfo.ConnType)
        {
            case ConnType.Socket:
                SocketConnInfo conn = null;
                conn = JsonSerializer.Deserialize<SocketConnInfo>(connInfo.ConnString, Options.CustomJsonSerializerOptions)
                          ?? throw new ArgumentNullException("conn");
                try
                {
                    _server = new EquipTcpServer(conn.Address, conn.Port, _connectionMultiplexer, _sqlSugarClient, _equipConnect, _mqttExplorer);
                    _server.Start();
                }
                catch (Exception)
                {
                    await CloseConnectionAsync();
                    throw;
                }
                break;
            default:
                return false;
        }

        return false;
    }

    public override Task SendDataAsync(byte[] buffer)
    {
        return Task.CompletedTask;
    }
}