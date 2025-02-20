using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using SqlSugar;
using StackExchange.Redis;

namespace Hgzn.Mes.Iot.EquipManager
{
    public abstract class EquipConnectorBase : IEquipConnector
    {
        protected EquipConnectorBase(
            IConnectionMultiplexer connectionMultiplexer,
            IMqttExplorer mqttExplorer,
            ISqlSugarClient sugarClient)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _mqttExplorer = mqttExplorer;
            _sqlSugarClient = sugarClient;
        }
        protected readonly IConnectionMultiplexer _connectionMultiplexer;
        protected readonly IMqttExplorer _mqttExplorer;
        protected readonly ISqlSugarClient _sqlSugarClient;
        protected EquipConnect _equipConnect;
        protected EquipConnType? _connType;
        protected string? _uri;

        public abstract Task CloseConnectionAsync();

        public abstract Task<bool> ConnectAsync(ConnInfo connInfo);

        public abstract Task SendDataAsync(byte[] buffer);

        public abstract Task StartAsync();

        public abstract Task StopAsync();

        public async Task UpdateStateAsync(ConnStateType stateType)
        {
            // 记录到redis服务器
            var database = _connectionMultiplexer.GetDatabase();
            var key = string.Format(CacheKeyFormatter.EquipState, _connType.ToString(), _uri);
            if (stateType == ConnStateType.Run)
                await database.StringSetAsync(key, (int)ConnStateType.Run);
            if (stateType == ConnStateType.Off || stateType == ConnStateType.Stop)
                await database.StringSetAsync(key, (int)ConnStateType.Off);

            await _mqttExplorer.PublishAsync(UserTopicBuilder
            .CreateUserBuilder()
            .WithPrefix(TopicType.App)
            .WithDirection(MqttDirection.Up)
            .WithTag(MqttTag.State)
            .WithUri(_uri!)
            .Build(), BitConverter.GetBytes((int)stateType));
        }

        public async Task UpdateOperationAsync(ConnStateType stateType)
        {
            EquipOperationStatus equipOperationStatus = EquipOperationStatus.Stopped;
            switch (stateType)
            {
                case ConnStateType.On:
                    equipOperationStatus = EquipOperationStatus.Paused; break;
                case ConnStateType.Off:
                    equipOperationStatus = EquipOperationStatus.Stopped; break;
                case ConnStateType.Run:
                    equipOperationStatus = EquipOperationStatus.Running; break;
                case ConnStateType.Stop:
                    equipOperationStatus = EquipOperationStatus.Paused; break;
                default:
                    break;
            }
            // 记录到redis服务器
            var database = _connectionMultiplexer.GetDatabase();
            var key = string.Format(CacheKeyFormatter.EquipOperationStatus, _uri);
            await database.StringSetAsync(key, (int)equipOperationStatus);
        }
    }
}
