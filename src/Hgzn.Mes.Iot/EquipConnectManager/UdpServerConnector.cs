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
using Microsoft.Extensions.Configuration;

public class UdpServerConnector : EquipConnectorBase
{
    // UDP不区分Server和Client,只是扮演的角色不同,Server多用于监听端口并接收数据
    private UdpClient _udpServer = null!;
    private IPEndPoint _localEndPoint = null!;
    private bool _isRunning = false;
    // 多少个转一个（从前端配置进行）
    private int? _forwardLength = 10;
    private int? _forwardNum = 1;
    private ISqlSugarClient _localsqlSugarClinet;

    public UdpServerConnector(
        IConnectionMultiplexer connectionMultiplexer,
        IMqttExplorer mqttExplorer,
        ISqlSugarClient sqlSugarClient,
        string uri, EquipConnType connType,
        IConfiguration configuration)
        : base(connectionMultiplexer, mqttExplorer, sqlSugarClient, uri, connType)
    {
        _forwardNum = _equipConnect?.ForwardRate ?? 1;

        // 从配置文件中读取数据库连接配置
        DbConnOptions localdbconfig = configuration.GetSection("DbConnIotOptions").Get<DbConnOptions>()!;
        _localsqlSugarClinet = new SqlSugarClient(SqlSugarContext.Build(localdbconfig));
    }

    public override async Task CloseConnectionAsync()
    {
        if (_udpServer != null)
        {
            try
            {
                var port = _localEndPoint?.Port ?? 0;
                LoggerAdapter.LogInformation($"正在关闭UDP连接，端口: {port}");
                
                // 先停止接收循环
                _isRunning = false;
                
                // 关闭并释放UDP客户端（这会中断正在等待的 ReceiveAsync）
                // Close() 会自动中断 ReceiveAsync() 并抛出 ObjectDisposedException
                _udpServer.Close();
                _udpServer.Dispose();
                _udpServer = null!;
                
                // 等待一小段时间让接收任务有机会处理 ObjectDisposedException 并退出
                await Task.Delay(100);
                
                LoggerAdapter.LogInformation($"UDP连接已关闭，端口: {port}");
            }
            catch (Exception ex)
            {
                LoggerAdapter.LogError($"关闭UDP连接时发生错误: {ex.Message}");
            }
        }
        await UpdateStateAsync(ConnStateType.Stop);
        await UpdateOperationAsync(ConnStateType.Stop);
    }

    public override async Task StartAsync(Guid uri)
    {
        if (_udpServer != null && _isRunning)
        {
            // 已经开始运行，更新状态为 Run
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
            // 检查当前连接是否已经在运行且配置相同
            if (_udpServer != null && _isRunning && _localEndPoint != null && _localEndPoint.Port == conn.Port)
            {
                LoggerAdapter.LogInformation($"UDP连接已存在且正常运行，端口 {conn.Port}，无需重新连接");
                return true;
            }

            // 如果存在旧连接但配置不同，或连接已停止，则关闭旧连接
            if (_udpServer != null)
            {
                LoggerAdapter.LogInformation($"关闭旧UDP连接以释放端口 {_localEndPoint?.Port ?? 0}，准备连接新端口 {conn.Port}");
                await CloseConnectionAsync();
                // 等待端口完全释放
                await Task.Delay(300);
            }

            // 创建UDP服务端
            _localEndPoint = new IPEndPoint(IPAddress.Any, conn.Port);
            
            // 先创建 UdpClient，然后设置 SO_REUSEADDR，再绑定端口
            var newUdpServer = new UdpClient();
            newUdpServer.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            newUdpServer.Client.Bind(_localEndPoint);
            
            // 只有成功绑定后才赋值给成员变量
            _udpServer = newUdpServer;
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
            // 如果异常发生，确保清理
            try
            {
                if (_udpServer != null)
                {
                    _isRunning = false;
                    _udpServer.Close();
                    _udpServer.Dispose();
                    _udpServer = null!;
                }
            }
            catch
            {
                // 忽略清理时的异常
            }
            
            await UpdateStateAsync(ConnStateType.Off);
            await UpdateOperationAsync(ConnStateType.Off);
            LoggerAdapter.LogError($"UDP Server start failed: {ex.Message}");
            return false;
        }
    }

    // UDP数据接收方法
    private async Task ReceiveDataAsync()
    {
        LoggerAdapter.LogInformation($"UDP接收循环已启动，端口: {_localEndPoint?.Port}");
        
        while (_isRunning && _udpServer != null)
        {
            try
            {
                var result = await _udpServer.ReceiveAsync();
                var remoteEndPoint = result.RemoteEndPoint;
                var buffer = result.Buffer;
                bool hasData = false;
                LoggerAdapter.LogInformation($"AG - 收到Udp数据,数据来自{remoteEndPoint}");
                // 在这里处理接收到的数据（示例：打印日志）
                // 获取报文数据
                byte[] newBuffer;
                DateTime time;
                uint bufferLength;
                hasData = ReceiveHelper.GetMessage(buffer, out bufferLength, out time, out newBuffer);
                if (hasData && newBuffer != null)
                {
                    LoggerAdapter.LogInformation($"AG - Udp数据内容: {BitConverter.ToString(buffer, 0, (int)bufferLength).Replace("-", " ")}");
                    // 获取部署在采集器里的数据库
                    LocalReceiveDispatch dispatch = new LocalReceiveDispatch(_equipConnect!.EquipId, _localsqlSugarClinet, _connectionMultiplexer, _mqttExplorer);
                    await dispatch.Handle(buffer);
                    // 调用基类或MQTT等方法转发数据
                    await ProcessDataAsync(buffer);
                }
            }
            catch (ObjectDisposedException)
            {
                // UDP客户端已被释放，这是正常的关闭流程
                LoggerAdapter.LogInformation("UDP连接已正常关闭");
                break;
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted || 
                                             ex.SocketErrorCode == SocketError.OperationAborted)
            {
                // 接收操作被中断或取消，这是正常的关闭流程
                LoggerAdapter.LogInformation("UDP接收操作被取消");
                break;
            }
            catch (Exception ex)
            {
                // 其他异常才记录为错误
                LoggerAdapter.LogError($"UDP receive error: {ex.GetType().Name} - {ex.Message}");
                
                // 如果 socket 已被关闭，退出循环
                if (_udpServer == null || !_isRunning)
                {
                    break;
                }
                
                // 短暂延迟后继续，避免错误循环占用过多CPU
                await Task.Delay(1000);
            }
        }
        
        LoggerAdapter.LogInformation($"UDP接收循环已退出，端口: {_localEndPoint?.Port}");
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
                            .WithUri(_equipConnect!.Id.ToString()).Build();

            // 使用支持断点续传的发布方法
            if (_mqttExplorer is Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport.IMqttExplorerWithOffline mqttWithOffline)
            {
                await mqttWithOffline.PublishWithOfflineSupportAsync(topic, buffer, priority: 0, maxRetryCount: 3);
            }
            else
            {
                await _mqttExplorer.PublishAsync(topic, buffer);
            }
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
    private IPEndPoint? _lastRemoteEndPoint = null;
}