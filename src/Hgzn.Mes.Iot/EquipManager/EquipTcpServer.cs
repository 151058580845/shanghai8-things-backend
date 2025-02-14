using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using NetCoreServer;
using SqlSugar;
using StackExchange.Redis;

namespace Hgzn.Mes.Iot.EquipManager
{
    public class EquipTcpServer : EquipConnectorBase
    {
        private readonly EquipConnect _equipConnect;
        private readonly ISqlSugarClient _sqlSugarClient;

        private readonly TcpServer _tcpServer;

        public EquipTcpServer(
            string address, int port, string uri,
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer,
            EquipConnect connect, 
            ISqlSugarClient client) : base(connectionMultiplexer, mqttExplorer)
        {
            _tcpServer = new TcpServer(address, port);
            _uri = uri;
            _sqlSugarClient = client;
            _equipConnect = connect;
        }

        protected TcpSession CreateSession()
        {
            return new TcpServerConnector(this, _equipConnect, _sqlSugarClient, _mqttExplorer);
        }


        public override async Task<bool> ConnectAsync(ConnInfo connInfo)
        {
            TcpClient tcpClient = new TcpClient(_tcpServer.Address, _tcpServer.Port);
            bool successConnect = tcpClient.ConnectAsync();
            if (!successConnect) return successConnect;
            await UpdateStateAsync(ConnStateType.On);
            return successConnect;
        }

        public override async Task CloseConnectionAsync()
        {
            if (_tcpServer.Stop())
            {
                await UpdateStateAsync(ConnStateType.Off);
            }
        }

        public override async Task StartAsync()
        {
            if (_tcpServer.IsStarted) return;
            if (_tcpServer.Start())
            {
                await UpdateStateAsync(ConnStateType.Run);
            }
        }

        public override async Task StopAsync()
        {
            if (!_tcpServer.IsStarted) return;
            if (_tcpServer.Stop())
            {
                await UpdateStateAsync(ConnStateType.Stop);
            }
        }

        public override Task SendDataAsync(byte[] buffer)
        {
            return Task.CompletedTask;
        }
    }
}
