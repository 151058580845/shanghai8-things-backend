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
                    LoggerAdapter.LogInformation($"收到的连接字符串是:{connectStr}");
                    JsonNode jn = JsonSerializer.Deserialize<JsonNode>(connectStr);
                    JsonNode? ip = jn["address"];
                    LoggerAdapter.LogInformation($"解析连接字符串中的地址是:{ip}");
                    string localIp = _configuration.GetValue<string>("LocalIpAddress");
                    LoggerAdapter.LogInformation($"方法1本机IP地址是:{localIp}");
                    if (localIp != null && ip != null && ip.ToString() == localIp)
                    {
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
                            case ConnType.UdpServer:
                                equipConnector = new UdpServerConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(), connType);
                                if (!Connections.TryAdd(id, equipConnector)) throw new Exception("equip exist");
                                break;
                            case ConnType.ModbusRtu:
                                equipConnector = new ModbusRTUConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(), connType);
                                if (!Connections.TryAdd(id, equipConnector)) throw new Exception("equip exist");
                                break;
                        }
                    }
                    break;
                case EquipConnType.RKServer:
                    // Mqtt会让所有IOT都进行连接,不是该ip的就会连接失败,重新更新UI,所以只能让指定IP的IOT进行连接
                    LoggerAdapter.LogInformation($"收到的连接字符串是:{connectStr}");
                    JsonNode rkjn = JsonSerializer.Deserialize<JsonNode>(connectStr);
                    JsonNode? rkip = rkjn["address"];
                    LoggerAdapter.LogInformation($"解析连接字符串中的地址是:{rkip}");
                    string rklocalIp = _configuration.GetValue<string>("LocalIpAddress");
                    if (rkip.ToString() == "127.0.0.1" || (rklocalIp != null && rkip != null && rkip.ToString() == rklocalIp))
                    {
                        equipConnector = new HygrographConnector(_connectionMultiplexer, _mqtt, _client, id.ToString(),
                            connType);
                        if (!Connections.TryAdd(id, equipConnector))
                            throw new Exception("equip exist");
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

        public string GetSpecificLocalIPAddresses()
        {
            var ipAddresses = new List<string>();
            try
            {

                var host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ipString = ip.ToString();
                        // 筛选以10.125.157开头的IP地址
                        if (ipString.StartsWith("10.125.157"))
                        {
                            ipAddresses.Add(ipString);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LoggerAdapter.LogInformation($"方法1异常:{e.Message}");
            }

            return ipAddresses.FirstOrDefault();
        }

        public string GetLinuxSpecificIPs()
        {
            var ipAddresses = new List<string>();

            try
            {
                // 执行hostname -I命令获取所有IP地址
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = "-c \"hostname -I\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd().Trim();
                process.WaitForExit();

                // 筛选10.125.157开头的IP
                ipAddresses = output.Split(' ')
                                  .Where(ip => ip.StartsWith("10.125.157"))
                                  .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"方法2获取IP地址失败: {ex.Message}");
            }

            return ipAddresses.FirstOrDefault();
        }

        public string GetLinuxSpecificIPsFromIpCommand()
        {
            var ips = new List<string>();

            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = "-c \"ip -o -4 addr show | awk '{print $4}' | cut -d'/' -f1 | grep '^10\\.125\\.157'\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                ips = output.Split('\n')
                         .Where(ip => !string.IsNullOrEmpty(ip))
                         .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"方法3获取IP地址失败: {ex.Message}");
            }

            return ips.FirstOrDefault();
        }


        public static string GetSpecificIPsFromNetworkConfig()
        {
            var ips = new List<string>();

            try
            {
                // 读取网络配置文件（路径可能因发行版而异）
                string[] configFiles = {
            "/etc/network/interfaces",
            "/etc/sysconfig/network-scripts/ifcfg-*",
            "/etc/netplan/*.yaml"
        };

                foreach (var file in configFiles.Where(File.Exists))
                {
                    string content = File.ReadAllText(file);
                    // 简单正则匹配IP地址（实际应用中需要更复杂的解析）
                    var matches = System.Text.RegularExpressions.Regex.Matches(
                        content, @"10\.125\.157\.\d{1,3}");
                    ips.AddRange(matches.Select(m => m.Value));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"方法4读取网络配置失败: {ex.Message}");
            }

            return ips.Distinct().ToList().FirstOrDefault();
        }
    }
}