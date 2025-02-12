using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Events;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Iot.EquipManager;
using MediatR;
using NetCoreServer;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.ProtocolManagers.TcpServer
{
    public class EquipTcpServer : NetCoreServer.TcpServer, IEquipConnector
    {
        private readonly List<TcpSession> _tcpSessions = [];
        // public DataProcessor DataProcessor { get; set; }
        private readonly Dictionary<Guid, IPAddress?> _sessionDictionary = new();
        private readonly string _heartBeatMessage;
        private readonly string _heartBeatAck;
        private readonly int _heartTime;
        private readonly EquipConnect _equipConnect;
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly IMqttExplorer _mqttExplorer;
        private readonly string _uri;

        public EquipTcpServer(IPAddress address, int port, string uri) : base(address, port)
        {
            _uri = uri;
        }

        public EquipTcpServer(string address, int port, string uri,
            EquipConnect connect, ISqlSugarClient client, IMqttExplorer mqttExplorer) : base(address, port)
        {
            _uri = uri;
            _sqlSugarClient = client;
            _mqttExplorer = mqttExplorer;
            _equipConnect = connect;
        }

        public EquipTcpServer(DnsEndPoint endpoint) : base(endpoint)
        {
        }

        public EquipTcpServer(IPEndPoint endpoint) : base(endpoint)
        {
        }

        protected override TcpSession CreateSession()
        {
            return new TcpServerConnector(this, _equipConnect, _sqlSugarClient, _mqttExplorer);
        }

        protected override async void OnConnected(TcpSession session)
        {
            // LogHelper.LogInformation("Tcpclient连接", session.Id);
            _tcpSessions.Add(session);
            // ((EquipTcpSession)session).DataProcessor = DataProcessor;
            base.OnConnected(session);
            var ip = session.Socket.RemoteEndPoint as IPEndPoint;
            EquipLedger equipLedger = await _sqlSugarClient.Queryable<EquipLedger>().FirstAsync(x => x.IpAddress == ip.ToString());
            await _sqlSugarClient.Updateable(equipLedger).ExecuteCommandAsync();
            _sessionDictionary.Add(session.Id, ip.Address);
        }

        protected async override void OnDisconnected(TcpSession session)
        {
            var sessionId = session.Id;
            _tcpSessions.Remove(session);
            base.OnDisconnected(session);

            IPAddress? ip = _sessionDictionary.GetValueOrDefault(sessionId);
            EquipLedger equipLedger = await _sqlSugarClient.Queryable<EquipLedger>().FirstAsync(x => x.IpAddress == ip.ToString());
            await _sqlSugarClient.Updateable(equipLedger).ExecuteCommandAsync();
        }

        public async Task<bool> ConnectAsync(ConnInfo connInfo)
        {
            await _mqttExplorer.UpdateStateAsync(ConnStateType.On, _uri);
            return true;
        }

        public async Task CloseConnectionAsync()
        {
            base.Stop();
            await _mqttExplorer.UpdateStateAsync(ConnStateType.Off, _uri);
        }

        public async Task StartAsync()
        {
            base.Start();
            await _mqttExplorer.UpdateStateAsync(ConnStateType.Run, _uri);
        }

        public async Task StopAsync()
        {
            base.Stop();
            await _mqttExplorer.UpdateStateAsync(ConnStateType.Stop, _uri);
        }

        public Task SendDataAsync(byte[] buffer)
        {
            return Task.CompletedTask;
        }
    }
}
