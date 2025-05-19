using System;
using System.Collections.Concurrent;
using System.Net;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.UserValue;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using MySqlX.XDevAPI;
using NetCoreServer;
using SqlSugar;
using StackExchange.Redis;
using static Org.BouncyCastle.Math.EC.ECCurve;
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
                byte[] newBuffer;
                _hasData = ReceiveHelper.GetMessage(buffer, size, out newBuffer);
                if (_hasData && newBuffer != null)
                {
                    if (_forwardLength != null && _forwardNum == _forwardLength)
                    {
                        var topic = IotTopicBuilder.CreateIotBuilder()
                                        .WithPrefix(TopicType.Iot)
                                        .WithDirection(MqttDirection.Up)
                                        .WithTag(MqttTag.Transmit)
                                        .WithDeviceType(EquipConnType.IotServer.ToString())
                                        .WithUri(_equipConnect.Id.ToString()).Build();
                        await _mqttExplorer.PublishAsync(topic, newBuffer);
                        _forwardNum = 0;
                    }
                    _forwardNum++;
                    // 本地解析资产编号和异常解析
                    // 这里要记录到数据采集器的数据库,而不是服务器的数据库
                    // 创建新的SqlSugarClient实例用于本地数据记录
                    SqlSugarClient localDbClient = new SqlSugarClient(SqlSugarContext.Build(ReceiveHelper.LOCALDBCONFIG));

                    LocalReceiveDispatch dispatch = new LocalReceiveDispatch(_equipConnect.EquipId, localDbClient, _connectionMultiplexer, _mqttExplorer);
                    await dispatch.Handle(newBuffer);
                }

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