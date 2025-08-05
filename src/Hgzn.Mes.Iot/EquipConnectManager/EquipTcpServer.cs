using System.Net;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Microsoft.IdentityModel.Logging;
using NetCoreServer;
using SqlSugar;
using StackExchange.Redis;

namespace Hgzn.Mes.Iot.EquipConnectManager;

public class EquipTcpServer : TcpServer
{
    private readonly List<TcpSession> _tcpSessions = [];
    private readonly Dictionary<Guid, IPAddress?> _sessionDictionary = new();
    private readonly string _heartBeatMessage;
    private readonly string _heartBeatAck;
    private readonly int _heartTime;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private ISqlSugarClient _sqlSugarClient;
    private EquipConnect _equipConnect;
    private IMqttExplorer _mqttExplorer;
    private ISqlSugarClient _localClient;

    public int? ForwardRate { get; set; }

    public EquipTcpServer(string address, int port,
        IConnectionMultiplexer connectionMultiplexer,
        ISqlSugarClient sqlSugarClient,
        EquipConnect equipConnect,
        IMqttExplorer mqttExplorer,
        ISqlSugarClient localClinet) : base(address, port)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _sqlSugarClient = sqlSugarClient;
        _equipConnect = equipConnect;
        _mqttExplorer = mqttExplorer;
        ForwardRate = equipConnect.ForwardRate;
        _localClient = localClinet;
    }

    protected override TcpSession CreateSession()
    {
        return new EquipTcpSession(this, _connectionMultiplexer, _localClient, _equipConnect, _mqttExplorer);
    }

    protected override void OnConnected(TcpSession session)
    {
        LoggerAdapter.LogInformation($"Tcpclient connected Id:{session.Id}");
        _tcpSessions.Add(session);
        base.OnConnected(session);
        var ip = session.Socket.RemoteEndPoint as IPEndPoint;

        _sessionDictionary.Add(session.Id, ip.Address);
    }

    protected override void OnDisconnected(TcpSession session)
    {
        LoggerAdapter.LogInformation($"Tcpclient disconnected Id:{session.Id}");
        var sessionId = session.Id;
        _tcpSessions.Remove(session);
        base.OnDisconnected(session);
    }
}