using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Iot.EquipManager;
using SqlSugar;
using StackExchange.Redis;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.Common;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;

public class UdpServerConnector : EquipConnectorBase
{
    // UDP不区分Server和Client,只是扮演的角色不同,Server多用于监听端口并接收数据
    private UdpClient _udpServer = null!;
    private IPEndPoint _localEndPoint = null!;
    private bool _isRunning = false;
    private EquipConnect _equipConnect = null!;
    // 多少个转一个（从前端配置进行）
    private int? _forwardLength = 10;
    private int? _forwardNum = 1;
    private ISqlSugarClient _localsqlSugarClinet;

    public UdpServerConnector(
        IConnectionMultiplexer connectionMultiplexer,
        IMqttExplorer mqttExplorer,
        ISqlSugarClient sqlSugarClient,
        string uri, EquipConnType connType)
        : base(connectionMultiplexer, mqttExplorer, sqlSugarClient, uri, connType)
    {
        LoggerAdapter.LogInformation("连接的数据库是" + sqlSugarClient.CurrentConnectionConfig.ConnectionString);
        _equipConnect = sqlSugarClient.Queryable<EquipConnect>()?.First(x => x.Id == Guid.Parse(uri))!;
        _forwardNum = _equipConnect?.ForwardRate.Value;
        _localsqlSugarClinet = new SqlSugarClient(SqlSugarContext.Build(ReceiveHelper.LOCALDBCONFIG));
    }

    public override async Task CloseConnectionAsync()
    {
        if (_udpServer != null)
        {
            _udpServer.Close();
            _isRunning = false;
        }
        await UpdateStateAsync(ConnStateType.Stop);
        await UpdateOperationAsync(ConnStateType.Stop);
    }

    public override async Task StartAsync(Guid uri)
    {
        if (_udpServer != null && _isRunning)
        {
            _isRunning = true;
            await UpdateStateAsync(ConnStateType.Run);
            await UpdateOperationAsync(ConnStateType.Run);
        }
    }

    public override async Task StopAsync(Guid uri)
    {
        await CloseConnectionAsync();
    }

    public override async Task<bool> ConnectAsync(ConnInfo connInfo)
    {
        LoggerAdapter.LogInformation("开始udp连接...");
        if (connInfo?.ConnString is null)
            throw new ArgumentNullException(nameof(connInfo));

        SocketConnInfo conn = JsonSerializer.Deserialize<SocketConnInfo>(
            connInfo.ConnString, Options.CustomJsonSerializerOptions)
            ?? throw new ArgumentNullException("conn");

        try
        {
            LoggerAdapter.LogInformation($"开启服务,监听的端口是{conn.Port}");
            // 创建UDP服务端
            _localEndPoint = new IPEndPoint(IPAddress.Any, conn.Port);
            _udpServer = new UdpClient(_localEndPoint);
            _isRunning = true;

            LoggerAdapter.LogInformation($"开始接收数据");
            // 启动异步接收（UDP不需要Accept，直接开始接收数据）
            _ = Task.Run(() => ReceiveDataAsync());

            await UpdateStateAsync(ConnStateType.On);
            await UpdateOperationAsync(ConnStateType.On);
            LoggerAdapter.LogInformation($"UDP Server started on 0.0.0.0:{conn.Port}");
            return true;
        }
        catch (Exception ex)
        {
            await CloseConnectionAsync();
            LoggerAdapter.LogError($"UDP Server start failed: {ex.Message}");
            return false;
        }
    }

    // UDP数据接收方法
    private async Task ReceiveDataAsync()
    {
        while (_isRunning)
        {
            try
            {
                LoggerAdapter.LogInformation("即将开始接收数据...");
                var result = await _udpServer.ReceiveAsync();
                LoggerAdapter.LogInformation("收到一组数据...");
                var remoteEndPoint = result.RemoteEndPoint;
                var buffer = result.Buffer;
                bool hasData = false;

                // 在这里处理接收到的数据（示例：打印日志）
                LoggerAdapter.LogInformation($"Received {buffer.Length} bytes from {remoteEndPoint}");
                // 获取报文数据
                byte[] newBuffer;
                DateTime time;
                hasData = ReceiveHelper.GetMessage(buffer, out time, out newBuffer);
                if (hasData && newBuffer != null)
                {
                    // 获取部署在采集器里的数据库
                    LocalReceiveDispatch dispatch = new LocalReceiveDispatch(_equipConnect.EquipId, _localsqlSugarClinet, _connectionMultiplexer, _mqttExplorer);
                    await dispatch.Handle(buffer);
                    // 调用基类或MQTT等方法转发数据
                    await ProcessDataAsync(buffer);
                }
            }
            catch (ObjectDisposedException)
            {
                // Socket已关闭（正常退出）
                break;
            }
            catch (Exception ex)
            {
                LoggerAdapter.LogError($"UDP receive error: {ex.Message}");
                await Task.Delay(1000); // 避免错误循环
            }
        }
    }

    private async Task ProcessDataAsync(byte[] buffer)
    {
        if (_forwardLength != null && _forwardNum != null && _forwardNum == _forwardLength)
        {
            var topic = IotTopicBuilder.CreateIotBuilder()
                            .WithPrefix(TopicType.Iot)
                            .WithDirection(MqttDirection.Up)
                            .WithTag(MqttTag.Transmit)
                            .WithDeviceType(EquipConnType.IotServer.ToString())
                            .WithUri(_equipConnect.Id.ToString()).Build();
            await _mqttExplorer.PublishAsync(topic, buffer);
            _forwardNum = 0;
        }
        _forwardNum++;
    }

    // UDP发送方法（需指定目标地址）
    public override async Task SendDataAsync(byte[] buffer)
    {
        if (!_isRunning || _udpServer == null)
        {
            LoggerAdapter.LogWarning("UDP Server is not running");
            return;
        }

        try
        {
            // 注意：UDP需要知道目标地址，这里需要根据业务逻辑获取目标Endpoint
            // 示例：假设目标地址已通过其他方式存储（如上次通信的客户端）
            if (_lastRemoteEndPoint != null)
            {
                await _udpServer.SendAsync(buffer, buffer.Length, _lastRemoteEndPoint);
            }
        }
        catch (Exception ex)
        {
            LoggerAdapter.LogError($"UDP send failed: {ex.Message}");
        }
    }

    // 可选：记录最后通信的客户端地址（用于回复）
    private IPEndPoint _lastRemoteEndPoint = null;
}