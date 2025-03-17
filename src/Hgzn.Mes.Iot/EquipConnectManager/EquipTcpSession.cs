using System;
using System.Collections.Concurrent;
using System.Net;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver;
using MySqlX.XDevAPI;
using NetCoreServer;
using SqlSugar;
using StackExchange.Redis;
using static Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.TestDataOnlineReceive;
using Buffer = System.Buffer;

namespace Hgzn.Mes.Iot.EquipConnectManager;

public class EquipTcpSession : TcpSession
{
    private readonly string _heartBeatMessage;

    private readonly string _heartBeatAck;

    // private readonly Timer _timer;
    private string Ip;
    private string Mac;
    // 多少个转一个（从前端配置进行）
    private int? _forwardLength = 10;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private ISqlSugarClient _sqlSugarClient;
    private EquipConnect _equipConnect;
    private IMqttExplorer _mqttExplorer;
    public EquipTcpSession(EquipTcpServer server,
        IConnectionMultiplexer connectionMultiplexer,
        ISqlSugarClient sqlSugarClient,
        EquipConnect equipConnect,
        IMqttExplorer mqttExplorer) : base(server)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _sqlSugarClient = sqlSugarClient;
        _equipConnect = equipConnect;
        _mqttExplorer = mqttExplorer;
        _forwardLength = server.ForwardRate;
    }

    private const int BodyStartIndex = 13;
    //缓存数据
    private readonly ConcurrentQueue<byte[]> _dataQueue = new();
    //接收数据就开启
    private volatile bool _hasData = false;
    private volatile int _forwardNum = 1;

    private async Task ProcessDataAsync(CancellationToken cancellationToken, long size)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_dataQueue.TryDequeue(out var buffer))
            {
                //标准头
                var header = buffer[0];
                if (header != 0x5A)
                {
                    LoggerAdapter.LogWarning("报头符错误。");
                    _hasData = false;
                    return;
                }

                //报文长度
                var messageLength = BitConverter.ToUInt32(buffer, 1);
                if (messageLength != size)
                {
                    LoggerAdapter.LogWarning($"报文长度错误：接收到的长度为 {size}，报文中声明的长度为 {messageLength}");
                    _hasData = false;
                    return;
                }

                //解析报文流水号（1字节）
                byte number = buffer[5];

                // 解析时间
                byte[] bYear = new byte[2];
                Buffer.BlockCopy(buffer, 6, bYear, 0, 2);
                ushort year = BitConverter.ToUInt16(bYear, 0);

                byte month = buffer[8];
                byte day = buffer[9];
                byte hour = buffer[10];
                byte minute = buffer[11];
                byte second = buffer[12];

                //报文数据
                var length = messageLength - 6;
                var newBuffer = new byte[length];
                Buffer.BlockCopy(buffer, BodyStartIndex, newBuffer, 0, newBuffer.Length);

                if (_forwardLength != null && _forwardNum == _forwardLength)
                {
                    var topic = IotTopicBuilder.CreateIotBuilder()
                                    .WithPrefix(TopicType.Iot)
                                    .WithDirection(MqttDirection.Up)
                                    .WithTag(MqttTag.Data)
                                    .WithDeviceType(EquipConnType.IotServer.ToString())
                                    .WithUri(_equipConnect.Id.ToString()).Build();
                    await _mqttExplorer.PublishAsync(topic, newBuffer);
                    _forwardNum = 0;
                }
                _forwardNum++;
                //本地解析资产编号和异常解析
                TestDataLocalReceive testDataReceive = new TestDataLocalReceive(_equipConnect.EquipId, _sqlSugarClient, _connectionMultiplexer, _mqttExplorer);
                string computerNum = await testDataReceive.Handle(newBuffer, true);
            }

            _hasData = false;
            return;
        }
    }

    protected override void OnConnected()
    {
        var ipEndPoint = Socket.RemoteEndPoint as IPEndPoint;
        if (ipEndPoint == null) return;
        Ip = ipEndPoint.ToString();
        Mac = ipEndPoint.Address.ToString();
        base.OnConnected();
        LoggerAdapter.LogTrace($"tcpclient {Ip} connected");
    }

    protected override async void OnReceived(byte[] buffer, long offset, long size)
    {
        LoggerAdapter.LogTrace($"buffer received {buffer} , size:{size} ");
        _dataQueue.Enqueue(buffer);
        if (!_hasData)
        {
            _hasData = true;
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            _ = Task.Run(async () => await ProcessDataAsync(token, size), token);
        }
    }
}