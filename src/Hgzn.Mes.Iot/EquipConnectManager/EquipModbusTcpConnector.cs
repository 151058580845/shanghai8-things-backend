using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipData;
using Hgzn.Mes.Domain.Entities.Equip.EquipDataPoint;
using Hgzn.Mes.Domain.Entities.System.Account;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Utilities;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Iot.EquipManager;
using Modbus.Device;
using Newtonsoft.Json;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using System.Timers;
using static Hgzn.Mes.Domain.Entities.Equip.EquipControl.CollectionConfig;

namespace Hgzn.Mes.Iot.EquipConnectManager
{
    public class EquipModbusTcpConnect : EquipConnectorBase
    {
        /// <summary>
        /// 定时器，5秒校验一次
        /// </summary>
        private readonly System.Timers.Timer _timer;
        private Dictionary<Guid, System.Timers.Timer> collectTimers = new();
        /// <summary>
        /// 失败重连5次
        /// </summary>
        private int _failedTimes = 5;
        /// <summary>
        /// 连接的取消
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource = new();
        private TcpClient? _tcpClient;
        private ModbusTcpConnInfo _modbusTcpConnInfo;
        private IModbusMaster? _modbusIpMaster;
        private static readonly ConcurrentDictionary<Guid, DataPointStatus> DataPointStatusDict = new();
        private readonly Dictionary<Guid, ModbusTcpDataPoint> _collectAddress = new();
        /// <summary>
        /// guid是EquipDataPointAggregateRoot的id
        /// </summary>
        protected readonly Dictionary<Guid, CancellationTokenSource> CancelTokens = new();
        protected EquipDataPoint DataPoint = new();

        public EquipConnect Connect { get; set; }
        public string ConnectStr { get; set; }

        public EquipModbusTcpConnect(IConnectionMultiplexer connectionMultiplexer, IMqttExplorer mqttExplorer, ISqlSugarClient sugarClient, string uri, EquipConnType connType) : base(connectionMultiplexer, mqttExplorer, sugarClient)
        {
            _uri = uri;
            _connType = connType;
            Connect = _sqlSugarClient.Queryable<EquipConnect>().Where(it => it.Id.ToString() == uri).First();
            if (Connect?.ConnectStr == null) return;
            ConnectStr = Connect.ConnectStr;
            _modbusTcpConnInfo = JsonConvert.DeserializeObject<ModbusTcpConnInfo>(ConnectStr)!;
            _timer = new System.Timers.Timer(5000);
            // _timer.Elapsed += CheckConnect;
            _timer.AutoReset = true;
        }

        public async override Task CloseConnectionAsync()
        {
            _timer.Stop();
            var thread = new Thread(async () => { await BaseDisConnectionAsync(); });
            thread.Start();

            await UpdateStateAsync(ConnStateType.Off);
        }

        public async override Task<bool> ConnectAsync(ConnInfo connInfo)
        {
            CancellationTokenSource = new CancellationTokenSource();
            var timeoutTask = Task.Delay(7000, CancellationTokenSource.Token);
            var connectionTask = BaseConnectionAsync();
            var completeTask = await Task.WhenAny(connectionTask, timeoutTask);
            if (completeTask == timeoutTask)
            {
                await CancellationTokenSource.CancelAsync();
                throw new TimeoutException("连接失败：连接超时");
            }
            else if (completeTask.Exception != null)
            {
                throw completeTask.Exception;
            }

            await UpdateStateAsync(ConnStateType.On);

            _timer.Start();
            return true;
        }

        public async override Task SendDataAsync(byte[] buffer)
        {
            await SendContentAsync(buffer);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer"></param>
        protected virtual async Task SendContentAsync(byte[] buffer)
        {
            await Task.CompletedTask;
        }

        public async override Task StartAsync(Guid uri)
        {
            DataPoint = await _sqlSugarClient.Queryable<EquipDataPoint>().Where(x => x.Id == uri).FirstAsync();
            ModbusTcpDataPoint mtdp = JsonConvert.DeserializeObject<ModbusTcpDataPoint>(DataPoint.CollectionAddressStr!)!;
            _collectAddress.TryAdd(DataPoint.Id, mtdp);

            var cancelToken = new CancellationTokenSource();
            if (!CancelTokens.TryAdd(DataPoint.Id, cancelToken))
                CancelTokens[DataPoint.Id] = cancelToken;
            await Task.Run(async () => await CollectDataAsync(DataPoint), cancelToken.Token);
        }

        public async Task StopAsync()
        {
            foreach (var cancellation in CancelTokens)
            {
                await StopAsync(cancellation.Key);
            }

            await CloseConnectionAsync();
        }

        /// <summary>
        /// 停止某个点的采集
        /// </summary>
        /// <param name="dataPointId"></param>
        public override async Task StopAsync(Guid dataPointId)
        {
            if (CancelTokens.TryGetValue(dataPointId, out var cancelToken))
            {
                if (!DataPointStatusDict.ContainsKey(dataPointId))
                    DataPointStatusDict.TryAdd(dataPointId, DataPointStatus.Paused);
                DataPointStatusDict[dataPointId] = DataPointStatus.Paused;
                await cancelToken.CancelAsync();

                _collectAddress.Remove(dataPointId);

                var database = _connectionMultiplexer.GetDatabase();
                var key2 = string.Format(CacheKeyFormatter.EquipDataPointOperationStatus, dataPointId);
                await database.StringSetAsync(key2, 0);
            }
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        protected async Task BaseConnectionAsync()
        {
            try
            {
                if (_tcpClient == null || (_tcpClient?.Connected ?? false))
                {
                    _tcpClient = new TcpClient();
                    _tcpClient.ReceiveTimeout = _modbusTcpConnInfo.ReceiveTimeout;
                    _tcpClient.SendTimeout = _modbusTcpConnInfo.SendTimeout;
                    _tcpClient.ConnectAsync(_modbusTcpConnInfo.Address, _modbusTcpConnInfo.Port).Wait(_modbusTcpConnInfo.SendTimeout);
                    _modbusIpMaster = ModbusIpMaster.CreateIp(_tcpClient);
                }
            }
            catch (Exception ex)
            {
                // 处理连接异常，例如记录日志或重试连接
                throw new InvalidOperationException("连接初始化失败。", ex);
            }
            await Task.CompletedTask;
        }

        /// <summary>
        /// 结束连接
        /// </summary>
        protected async Task BaseDisConnectionAsync()
        {
            foreach (var cancelToken in CancelTokens)
            {
                await cancelToken.Value.CancelAsync();
            }

            _tcpClient?.Close();
            _modbusIpMaster?.Dispose();
            //_dataPoint.Clear();
        }

        /// <summary>
        /// 采集数据
        /// </summary>
        /// <param name="dataPoint"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private async Task CollectDataAsync(EquipDataPoint dataPoint)
        {
            try
            {
                if (!DataPointStatusDict.ContainsKey(dataPoint.Id))
                    DataPointStatusDict.TryAdd(dataPoint.Id, DataPointStatus.Progress);
                DataPointStatusDict[dataPoint.Id] = DataPointStatus.Progress;
                var collectAddress = _collectAddress[dataPoint.Id];
                var token = CancelTokens[dataPoint.Id];
                var database = _connectionMultiplexer.GetDatabase();
                var key = string.Format(CacheKeyFormatter.EquipDataPointStatus, EquipConnType.IotServer.ToString(), dataPoint.Id, DataPoint.Code);
                var key2 = string.Format(CacheKeyFormatter.EquipDataPointOperationStatus, dataPoint.Id);
                await database.StringSetAsync(key2, 1);
                while (!token.IsCancellationRequested)
                {
                    ushort[] datas = new ushort[collectAddress.ReadLength];
                    switch (collectAddress.ModbusReadType)
                    {
                        case ModbusReadType.ReadCoil:
                            var data = await _modbusIpMaster.ReadCoilsAsync(_modbusTcpConnInfo.SlaveId,
                                collectAddress.Address,
                                collectAddress.ReadLength);
                            await database.StringSetAsync(key, string.Join(",", data));
                            break;
                        case ModbusReadType.ReadDiscrete:
                            break;
                        case ModbusReadType.ReadInput:
                            datas = await _modbusIpMaster.ReadInputRegistersAsync(_modbusTcpConnInfo.SlaveId,
                                collectAddress.Address,
                                collectAddress.ReadLength);
                            await database.StringSetAsync(key, string.Join(",", datas));
                            break;
                        case ModbusReadType.ReadHoldingRegister:
                            datas = await _modbusIpMaster.ReadHoldingRegistersAsync(
                                _modbusTcpConnInfo.SlaveId,
                                collectAddress.Address,
                                collectAddress.ReadLength);
                            await database.StringSetAsync(key, string.Join(",", datas));
                            break;
                    }

                    // 根据 ReadTypeEnum 处理数据
                    switch (collectAddress.ReadTypeEnum)
                    {
                        case DataReadTypeEnum.Int:
                            var intValue1 = ModbusHelper.ConvertToInt32(datas, DataOrderType.ABCD);
                            Console.WriteLine(intValue1);
                            // 处理 intValue，例如存储或传递给其他系统
                            break;
                        case DataReadTypeEnum.UInt:
                            var stringValue = ModbusHelper.ConvertToUInt32(datas, _modbusTcpConnInfo.DataType);
                            // 处理 stringValue
                            Console.WriteLine(stringValue);
                            break;
                        // 处理其他数据类型
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    // 等待指定的采集频率再进行下一次读取
                    await Task.Delay(1000, token.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // 任务被取消，正常退出
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // 处理其他异常，例如记录日志
            }
        }

        /// <summary>
        /// 定时器自动判断是否连接中，自动重连5次
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckConnect(object? sender, ElapsedEventArgs e)
        {
            try
            {
                if (_failedTimes < 0)
                {
                    _timer.Stop();
                    return;
                }
                if (await IsConnectedAsync())
                {
                    return;
                }
                //再次连接尝试
                await CloseConnectionAsync();
                await BaseDisConnectionAsync();
                //将失败次数还原
                _failedTimes = 5;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                _failedTimes--;
            }
        }

        /// <summary>
        /// 当前采集配置连接状态
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsConnectedAsync()
        {
            return await Task.FromResult(_tcpClient?.Connected ?? false);
        }

        private JsonNode? GetJsonNode(string conStr)
        {
            JsonNode? jn = JsonNode.Parse(conStr);
            return jn;
        }
    }
}
