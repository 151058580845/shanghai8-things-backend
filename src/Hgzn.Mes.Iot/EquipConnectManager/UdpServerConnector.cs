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
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_121.ZXWL_SL_3;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_109.ZXWL_SL_3;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver;

public class UdpServerConnector : EquipConnectorBase
{
    // UDP不区分Server和Client,只是扮演的角色不同,Server多用于监听端口并接收数据
    private UdpClient _udpServer = null!;
    private IPEndPoint _localEndPoint = null!;
    private bool _isRunning = false;
    private EquipConnect _equipConnect = null!;

    public UdpServerConnector(
        IConnectionMultiplexer connectionMultiplexer,
        IMqttExplorer mqttExplorer,
        ISqlSugarClient sqlSugarClient,
        string uri, EquipConnType connType)
        : base(connectionMultiplexer, mqttExplorer, sqlSugarClient, uri, connType)
    {
        _equipConnect = sqlSugarClient.Queryable<EquipConnect>().First(x => x.Id == Guid.Parse(uri));
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
        if (connInfo?.ConnString is null)
            throw new ArgumentNullException(nameof(connInfo));

        SocketConnInfo conn = JsonSerializer.Deserialize<SocketConnInfo>(
            connInfo.ConnString, Options.CustomJsonSerializerOptions)
            ?? throw new ArgumentNullException("conn");

        try
        {
            // 创建UDP服务端
            _localEndPoint = new IPEndPoint(IPAddress.Any, conn.Port);
            _udpServer = new UdpClient(_localEndPoint);
            _isRunning = true;

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
                var result = await _udpServer.ReceiveAsync();
                var remoteEndPoint = result.RemoteEndPoint;
                var buffer = result.Buffer;

                // 在这里处理接收到的数据（示例：打印日志）
                LoggerAdapter.LogInformation($"Received {buffer.Length} bytes from {remoteEndPoint}");
                // 获取报文数据
                byte[] newBuffer;
                ReceiveHelper.GetMessage(buffer, buffer.Length, out newBuffer);
                // 获取部署在采集器里的数据库
                SqlSugarClient localDbClient = new SqlSugarClient(SqlSugarContext.Build(ReceiveHelper.LOCALDBCONFIG));
                LocalReceiveDispatch dispatch = new LocalReceiveDispatch(_equipConnect.EquipId, localDbClient, _connectionMultiplexer, _mqttExplorer);
                await dispatch.Handle(newBuffer);

                // 可以调用基类或MQTT等方法转发数据
                // await ProcessReceivedDataAsync(buffer, remoteEndPoint);
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