using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Iot.EquipManager;
using SqlSugar;
using StackExchange.Redis;
using System.Text.Json;

namespace Hgzn.Mes.Iot.EquipConnectManager
{
    public class TcpServerConnector : EquipConnectorBase
    {
        private EquipTcpServer _server = null!;

        public TcpServerConnector(
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer,
            ISqlSugarClient sqlSugarClient,
            string uri, EquipConnType connType) :
            base(connectionMultiplexer, mqttExplorer, sqlSugarClient, uri, connType)
        {            
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
            SocketConnInfo conn = null;
            conn = JsonSerializer.Deserialize<SocketConnInfo>(connInfo.ConnString, Options.CustomJsonSerializerOptions) ?? throw new ArgumentNullException("conn");
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

            return false;
        }

        public override Task SendDataAsync(byte[] buffer)
        {
            return Task.CompletedTask;
        }
    }
}
