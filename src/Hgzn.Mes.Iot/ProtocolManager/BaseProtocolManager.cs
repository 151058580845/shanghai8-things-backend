using System.Timers;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Shared.Enums;
using Timer = System.Timers.Timer;

namespace Hgzn.Mes.Iot.ProtocolManager;

public abstract class BaseProtocolManager : IEquipManager
{
    /// <summary>
    /// 连接的取消
    /// </summary>
    protected CancellationTokenSource CancellationTokenSource = new();
    
    /// <summary>
    /// 当前的连接编号
    /// </summary>
    public EquipConnect EquipConnect { get; set; }
    
    /// <summary>
    /// guid是EquipData的id
    /// </summary>
    protected readonly Dictionary<Guid, CancellationTokenSource> CancelTokens = new();
    
    /// <summary>
    /// 定时器，5秒校验一次
    /// </summary>
    private readonly Timer _timer;
    /// <summary>
    /// 失败重连5次
    /// </summary>
    private int _failedTimes = 5;
    public BaseProtocolManager()
    {
        _timer = new Timer(5000);
        _timer.Elapsed += CheckConnect;
        _timer.AutoReset = true;
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
            await DisConnectionAsync();
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
    /// 修改采集参数
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public abstract Task UpdateConnectionParameter(string parameter);

    /// <summary>
    /// 设备连接
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="AggregateException"></exception>
    public async Task ConnectionAsync()
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

        if (completeTask.Exception != null)
        {
            throw completeTask.Exception;
        }

        // await BaseConnectionAsync();
        await SendEventConn(EquipConnect.Id, EquipConnectEnum.Connect);
        _timer.Start();
    }
    
    /// <summary>
    /// 设备连接
    /// </summary>
    /// <returns></returns>
    protected abstract Task BaseConnectionAsync();
    /// <summary>
    /// 设备取消连接
    /// </summary>
    /// <returns></returns>
    public async Task DisConnectionAsync()
    {
        _timer.Stop();
        var thread = new Thread(() => { BaseDisConnectionAsync(); });
        thread.Start();
        await SendEventConn(EquipConnect.Id, EquipConnectEnum.DisConnect);
    }
    /// <summary>
    /// 设备断开连接
    /// </summary>
    /// <returns></returns>
    protected abstract Task BaseDisConnectionAsync();
    /// <summary>
    /// 全部开始采集
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task StartAsync()
    {
        await ConnectionAsync();
        foreach (var cancellation in CancelTokens)
        {
            await StartAsync(cancellation.Key);
        }
    }
    /// <summary>
    /// 某个点位开始采集
    /// </summary>
    /// <param name="dataPointId"></param>
    /// <returns></returns>
    public abstract Task StartAsync(Guid dataPointId);

    /// <summary>
    /// 全部停止采集
    /// </summary>
    public virtual async Task StopAsync()
    {
        foreach (var cancellation in CancelTokens)
        {
            await StopAsync(cancellation.Key);
        }

        await DisConnectionAsync();
    }

    /// <summary>
    /// 停止某个点的采集
    /// </summary>
    /// <param name="dataPointId"></param>
    public virtual async Task StopAsync(Guid dataPointId)
    {
        if (CancelTokens.TryGetValue(dataPointId, out var cancelToken))
        {
            // EquipControlHelp.SetStatusByPointId(dataPointId, DataPointStatus.Paused);
            await cancelToken.CancelAsync();
        }
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    /// <returns></returns>
    public virtual async Task<bool> TestConnectionAsync()
    {
        await ConnectionAsync();
        await DisConnectionAsync();
        return true;
    }

    public async Task SendDataAsync(byte[] buffer)
    {
        await SendContentAsync(buffer);
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="buffer"></param>
    protected virtual Task SendContentAsync(byte[] buffer)
    {
        return Task.CompletedTask;
    }

    public abstract Task UpdateEquipConnectForward(List<Guid> targetIds);

    public abstract Task<bool> IsConnectedAsync();
    
    /// <summary>
    /// 发送连接事件
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="connectEnum"></param>
    private async Task SendEventConn(Guid connectionId, EquipConnectEnum connectEnum)
    {
        // await LocalEventBus.PublishAsync(new EquipCommonConnectEventArgs()
        // {
        //     ConnectId = connectionId,
        //     ConnectEnum = connectEnum
        // });
    }
}