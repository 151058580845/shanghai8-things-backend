using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json.Nodes;

namespace Hgzn.Mes.Iot.EquipManager
{
    public class ConnManager
    {
        private IMqttExplorer _mqtt = null!;
        private readonly ISqlSugarClient _client;
        private readonly IMqttExplorer _mqttExplorer;

        public static ConcurrentDictionary<Guid, IEquipConnector> Connections { get; private set; } = new();

        public ConnManager(ISqlSugarClient client)
        {
            this._client = client;
        }

        public void Initialize(IMqttExplorer mqttExplorer)
        {
            _mqtt = mqttExplorer;
        }

        public async Task<IEquipConnector> AddEquip(Guid id, string equipType, string connectStr)
        {
            if (Connections.TryGetValue(id, out var value))
            {
                return value;
            }   
            IEquipConnector? connector = null;
            switch (equipType)
            {
                case "rfid-reader":
                    connector = new RfidReaderConnector(_mqtt, id.ToString(), equipType);
                    if (!Connections.TryAdd(id, connector))
                        throw new Exception("equip exist");
                    break;
                case "tcp-server":
                    EquipConnect connect = await _client.Queryable<EquipConnect>().FirstAsync(x => x.Id == id);
                    JsonNode? jn = GetJsonNode(connectStr);
                    if (jn == null) break;
                    string? address = jn["address"]?.ToString();
                    if (address == null || !int.TryParse(jn["port"]?.ToString(), out int port)) break;
                    connector = new EquipTcpServer(address, port, id.ToString(), connect, _client, _mqtt);
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
