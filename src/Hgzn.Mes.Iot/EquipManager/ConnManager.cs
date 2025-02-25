using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json.Nodes;
using StackExchange.Redis;

namespace Hgzn.Mes.Iot.EquipManager
{
    public class ConnManager
    {
        private IMqttExplorer _mqtt = null!;
        private readonly ISqlSugarClient _client;
        private IConnectionMultiplexer _connectionMultiplexer;
        public static ConcurrentDictionary<Guid, IEquipConnector> Connections { get; private set; } = new();

        public ConnManager(
            ISqlSugarClient client,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _client = client;
            _connectionMultiplexer = connectionMultiplexer;
        }

        /// <summary>
        /// 因为有循环依赖所以不放在构造函数里注入
        /// </summary>
        /// <param name="mqttExplorer"></param>
        public void Initialize(IMqttExplorer mqttExplorer)
        {
            _mqtt = mqttExplorer;
        }

        public IEquipConnector AddEquip(Guid id, EquipConnType connType, string connectStr)
        {
            if (Connections.TryGetValue(id, out var value))
            {
                return value;
            }

            IEquipConnector? connector;
            switch (connType)
            {
                case EquipConnType.RfidReader:
                    connector = new RfidReaderConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(),
                        connType);
                    if (!Connections.TryAdd(id, connector))
                        throw new Exception("equip exist");
                    break;
                case EquipConnType.IotServer:
                    connector = new TcpServer.TcpServerConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(),
                        connType);
                    if (!Connections.TryAdd(id, connector))
                        throw new Exception("equip exist");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("equipType");
            }
            return connector ?? throw new ArgumentNullException();
        }

        public IEquipConnector? GetEquip(Guid id)
        {
            if (Connections.TryGetValue(id, out var connector))
            {
                return connector;
            }

            return null;
        }

        public JsonNode? GetJsonNode(string conStr)
        {
            JsonNode? jn = JsonNode.Parse(conStr);
            return jn;
        }
    }
}