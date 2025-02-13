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
        private readonly string _heartBeatMessage;
        private readonly string _heartBeatAck;
        private readonly int _heartTime;
        private readonly EquipConnect _equipConnect;
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly IMqttExplorer _mqttExplorer;
        private readonly string _uri;

        public EquipTcpServer(string address, int port, string uri,
            EquipConnect connect, ISqlSugarClient client, IMqttExplorer mqttExplorer) : base(address, port)
        {
            _uri = uri;
            _sqlSugarClient = client;
            _mqttExplorer = mqttExplorer;
            _equipConnect = connect;
        }

        protected override TcpSession CreateSession()
        {
            return new TcpServerConnector(this, _equipConnect, _sqlSugarClient, _mqttExplorer);
        }

        protected override void OnConnected(TcpSession session)
        {
            _tcpSessions.Add(session);
            base.OnConnected(session);
        }

        protected override void OnDisconnected(TcpSession session)
        {
            _tcpSessions.Remove(session);
            base.OnDisconnected(session);
        }

        public async Task<bool> ConnectAsync(ConnInfo connInfo)
        {
            TcpClient tcpClient = new TcpClient(Address, Port);
            bool successConnect = tcpClient.ConnectAsync();
            if (!successConnect) return successConnect;
            await _mqttExplorer.UpdateStateAsync(ConnStateType.On, _uri);
            return successConnect;
        }

        public async Task CloseConnectionAsync()
        {
            if (base.Stop())
            {
                await _mqttExplorer.UpdateStateAsync(ConnStateType.Off, _uri);
            }
        }

        public async Task StartAsync()
        {
            if (IsStarted) return;
            if (base.Start())
            {
                await _mqttExplorer.UpdateStateAsync(ConnStateType.Run, _uri);
            }
        }

        public async Task StopAsync()
        {
            if (!IsStarted) return;
            if (base.Stop())
            {
                await _mqttExplorer.UpdateStateAsync(ConnStateType.Stop, _uri);
            }
        }

        public Task SendDataAsync(byte[] buffer)
        {
            return Task.CompletedTask;
        }
    }
}
