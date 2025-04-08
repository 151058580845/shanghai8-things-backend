using System.Text.Json;
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
            ISqlSugarClient sugarClient,
            string uri, EquipConnType connType)
        {
            _uri = uri;
            _connType = connType;
            _connectionMultiplexer = connectionMultiplexer;
            _mqttExplorer = mqttExplorer;
            _sqlSugarClient = sugarClient;
            _equipConnect = sugarClient.Queryable<EquipConnect>().First(x => x.Id == Guid.Parse(_uri));
        }
        protected readonly IConnectionMultiplexer _connectionMultiplexer;
        protected readonly IMqttExplorer _mqttExplorer;
        protected readonly ISqlSugarClient _sqlSugarClient;
        protected EquipConnect? _equipConnect;
        protected EquipConnType? _connType;
        protected string? _uri;

        public abstract Task CloseConnectionAsync();

        public abstract Task<bool> ConnectAsync(ConnInfo connInfo);

        public abstract Task SendDataAsync(byte[] buffer);

        public abstract Task StartAsync(Guid uri);

        public abstract Task StopAsync(Guid uri);

        public async Task UpdateStateAsync(ConnStateType stateType)
        {
            // 记录到redis服务器
            var database = _connectionMultiplexer.GetDatabase();
            var key = string.Format(CacheKeyFormatter.EquipState, _equipConnect.EquipId, _uri);
            await database.StringSetAsync(key, (int)stateType);
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

        public static JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
