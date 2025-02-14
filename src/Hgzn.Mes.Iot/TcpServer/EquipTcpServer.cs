using System.Net;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Microsoft.IdentityModel.Logging;
using NetCoreServer;
using SqlSugar;

namespace Hgzn.Mes.Iot.TcpServer;

public class EquipTcpServer : NetCoreServer.TcpServer
{
    private readonly List<TcpSession> _tcpSessions = [];
    private readonly Dictionary<Guid, IPAddress?> _sessionDictionary = new();
    private readonly string _heartBeatMessage;
    private readonly string _heartBeatAck;
    private readonly int _heartTime;
    private ISqlSugarClient _sqlSugarClient;
    private EquipConnect _equipConnect;
    private IMqttExplorer _mqttExplorer;
    
    public EquipTcpServer(string address, int port,
        ISqlSugarClient sqlSugarClient,
        EquipConnect equipConnect,
        IMqttExplorer mqttExplorer) : base(address, port)
    {
        _sqlSugarClient = sqlSugarClient;
        _equipConnect = equipConnect;
        _mqttExplorer = mqttExplorer;
    }

    protected override TcpSession CreateSession()
    {
        return new EquipTcpSession(this,_sqlSugarClient,_equipConnect,_mqttExplorer);
    }

    protected override void OnConnected(TcpSession session)
    {
        LogHelper.LogInformation("Tcpclient连接", session.Id);
        _tcpSessions.Add(session);
        base.OnConnected(session);
        var ip = session.Socket.RemoteEndPoint as IPEndPoint;

        _sessionDictionary.Add(session.Id, ip.Address);
    }

    protected override void OnDisconnected(TcpSession session)
    {
        var sessionId = session.Id;
        _tcpSessions.Remove(session);
        base.OnDisconnected(session);
    }
}