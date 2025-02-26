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
using Hgzn.Mes.Iot.EquipConnectManager;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using System.Collections.Generic;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;

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

        public IEquipConnector AddEquip(Guid id, EquipConnType connType, string connectStr, ConnInfo connectInfo)
        {
            if (Connections.TryGetValue(id, out var value))
            {
                return value;
            }

            IEquipConnector? equipConnector = null;
            switch (connType)
            {
                case EquipConnType.RfidReader:
                    equipConnector = new RfidReaderConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(),
                        connType);
                    if (!Connections.TryAdd(id, equipConnector))
                        throw new Exception("equip exist");
                    break;
                case EquipConnType.IotServer:
                    switch (connectInfo.ConnType)
                    {
                        case ConnType.Socket:
                            equipConnector = new TcpServerConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(), connType);
                            if (!Connections.TryAdd(id, equipConnector)) throw new Exception("equip exist");
                            break;
                        case ConnType.ModbusTcp:
                            equipConnector = new EquipModbusTcpConnect(_connectionMultiplexer, _mqtt, _client, id.ToString(), connType);
                            if (!Connections.TryAdd(id, equipConnector)) throw new Exception("equip exist");
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("equipType");
            }
            return equipConnector ?? throw new ArgumentNullException();
        }

        public IEquipConnector? GetEquip(Guid id)
        {
            if (Connections.TryGetValue(id, out var connector))
            {
                return connector;
            }

            return null;
        }
    }
}