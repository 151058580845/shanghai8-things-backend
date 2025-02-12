using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Events;
using Hgzn.Mes.Domain.Shared.Enums;
using MediatR;
using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hgzn.Mes.Domain.ProtocolManagers.TcpServer
{
    public class EquipTcpServer : NetCoreServer.TcpServer
    {
        private readonly List<TcpSession> _tcpSessions = [];
        // public DataProcessor DataProcessor { get; set; }
        private readonly IMediator _mediator;
        private readonly Dictionary<Guid, IPAddress?> _sessionDictionary = new();
        private readonly string _heartBeatMessage;
        private readonly string _heartBeatAck;
        private readonly int _heartTime;
        private readonly EquipConnect _equipConnect;
        public EquipTcpServer(IPAddress address, int port, IMediator mediator) : base(address, port)
        {
            _mediator = mediator;
        }

        public EquipTcpServer(string address, int port, string heartBeatMessage, string heartBeatAck, int heartTime,
            IMediator mediator, EquipConnect connectAggregate) : base(address, port)
        {
            _mediator = mediator;
            _heartBeatMessage = heartBeatMessage;
            _heartBeatAck = heartBeatAck;
            _heartTime = heartTime;
            _equipConnect = connectAggregate;
        }

        public EquipTcpServer(DnsEndPoint endpoint) : base(endpoint)
        {
        }

        public EquipTcpServer(IPEndPoint endpoint) : base(endpoint)
        {
        }

        protected override TcpSession CreateSession()
        {
            return new EquipTcpSession(this, _heartBeatMessage, _heartBeatAck, _heartTime, _mediator, _equipConnect);
        }

        protected override void OnConnected(TcpSession session)
        {
            // LogHelper.LogInformation("Tcpclient连接", session.Id);
            _tcpSessions.Add(session);
            // ((EquipTcpSession)session).DataProcessor = DataProcessor;
            base.OnConnected(session);
            var ip = session.Socket.RemoteEndPoint as IPEndPoint;
            _mediator.Publish(new EquipTcpConnectNotification()
            {
                SessionId = session.Id,
                IpAddress = ip.Address,
                MacAddress = "",
                ConnectEnum = EquipConnectEnum.Connect
            });
            _sessionDictionary.Add(session.Id, ip.Address);
        }

        protected override void OnDisconnected(TcpSession session)
        {
            var sessionId = session.Id;
            _tcpSessions.Remove(session);
            base.OnDisconnected(session);
            _mediator.Publish(new EquipTcpConnectNotification()
            {
                SessionId = sessionId,
                IpAddress = _sessionDictionary.GetValueOrDefault(sessionId),
                MacAddress = "",
                ConnectEnum = EquipConnectEnum.DisConnect
            });
        }
    }
}
