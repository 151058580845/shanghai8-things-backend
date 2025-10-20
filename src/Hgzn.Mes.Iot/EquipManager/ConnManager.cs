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
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Net.Sockets;
using Hgzn.Mes.Domain.ValueObjects.Message.Base;
using System.Text.Json;
using Hgzn.Mes.Domain.Shared;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Hgzn.Mes.Iot.EquipManager
{
    public class ConnManager
    {
        private IMqttExplorer _mqtt = null!;
        private readonly ISqlSugarClient _client;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IConfiguration _configuration;
        public ConcurrentDictionary<Guid, IEquipConnector> Connections { get; private set; } = new();

        public ConnManager(
            ISqlSugarClient client,
            IConnectionMultiplexer connectionMultiplexer,
            IConfiguration configuration)
        {
            _client = client;
            _connectionMultiplexer = connectionMultiplexer;
            _configuration = configuration;
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
                (_mqtt.RestartAsync()).Wait();
                return value;
            }

            IEquipConnector? equipConnector = null;
            switch (connType)
            {
                case EquipConnType.RfidReader:
                    var interval = _configuration.GetValue<int>("PushInterval");
                    equipConnector = new RfidReaderConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(),
                        connType, interval);
                    if (!Connections.TryAdd(id, equipConnector))
                        throw new Exception("equip exist");
                    break;
                case EquipConnType.IotServer:
                    // Mqtt会让所有IOT都进行连接,不是该ip的就会连接失败,重新更新UI,所以只能让指定IP的IOT进行连接
                    JsonNode jn = JsonSerializer.Deserialize<JsonNode>(connectStr);
                    JsonNode? ip = jn["address"];
                    LoggerAdapter.LogInformation($"AG - 解析连接字符串中的地址是:{ip}");
                    string localIp = _configuration.GetValue<string>("LocalIpAddress");
                    LoggerAdapter.LogInformation($"AG - 本机配置的IP地址是:{localIp}");
                    if (localIp != null && ip != null && ip.ToString() == localIp)
                    {
                        switch (connectInfo.ConnType)
                        {
                            case ConnType.TcpServer:
                                equipConnector = new TcpServerConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(), connType);
                                if (!Connections.TryAdd(id, equipConnector)) throw new Exception("equip exist");
                                break;
                            case ConnType.ModbusTcp:
                                equipConnector = new EquipModbusTcpConnect(_connectionMultiplexer, _mqtt, _client, id.ToString(), connType);
                                if (!Connections.TryAdd(id, equipConnector)) throw new Exception("equip exist");
                                break;
                            case ConnType.UdpServer:
                                equipConnector = new UdpServerConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(), connType);
                                if (!Connections.TryAdd(id, equipConnector)) throw new Exception("equip exist");
                                break;
                            case ConnType.ModbusRtu:
                                equipConnector = new ModbusRTUConnector(_connectionMultiplexer, _mqtt, _client, _configuration, id.ToString(), connType);
                                if (!Connections.TryAdd(id, equipConnector)) throw new Exception("equip exist");
                                break;
                        }
                    }
                    break;
                case EquipConnType.RKServer:
                    // Mqtt会让所有IOT都进行连接,不是该ip的就会连接失败,重新更新UI,所以只能让指定IP的IOT进行连接
                    JsonNode rkjn = JsonSerializer.Deserialize<JsonNode>(connectStr);
                    JsonNode? rkip = rkjn["address"];
                    LoggerAdapter.LogInformation($"AG - 温湿度计连接 - 解析连接字符串中的地址是:{rkip}");
                    string rklocalIp = _configuration.GetValue<string>("LocalIpAddress");
                    LoggerAdapter.LogInformation($"AG - 温湿度计连接 - 本机配置的IP地址是:{rklocalIp}或127.0.0.1");
                    if (rkip.ToString() == "127.0.0.1" || (rklocalIp != null && rkip != null && rkip.ToString() == rklocalIp))
                    {
                        LoggerAdapter.LogInformation($"AG - 温湿度计连接 - IP地址匹配成功，创建HygrographConnector");
                        equipConnector = new HygrographConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(),
                            connType);
                        if (!Connections.TryAdd(id, equipConnector))
                            throw new Exception("equip exist");
                    }
                    else
                    {
                        LoggerAdapter.LogWarning($"AG - 温湿度计连接 - IP地址不匹配，跳过连接器创建。期望:{rklocalIp}或127.0.0.1，实际:{rkip}");
                    }
                    break;
                case EquipConnType.CardIssuer:
                    var interval2 = _configuration.GetValue<int>("PushInterval");
                    equipConnector = new CardIssuerConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(),
                        connType, interval2);
                    if (!Connections.TryAdd(id, equipConnector))
                        throw new Exception("equip exist");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("equipType");
            }

            if (equipConnector == null)
                LoggerAdapter.LogDebug($"创建连接器失败!");
            else
                LoggerAdapter.LogDebug($"创建连接器成功!");
            return equipConnector!;
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