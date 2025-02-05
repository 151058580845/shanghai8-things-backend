using System.Collections.Concurrent;
using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Iot.ProtocolManager;

public static class EquipControlHelp
{
    private static readonly ConcurrentDictionary<Guid, IEquipManager> EquipManagers = new();
    private static readonly ConcurrentDictionary<Guid, DataPointStatus> DataPointStatusDict = new();
    private static readonly ConcurrentDictionary<Guid, List<Guid>> ForwardConnectList = new();

    /// <summary>
    /// 返回所有下一条地址
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    public static async Task<List<Guid>> GetAllForwardConnectListAsync(Guid connectionId)
    {
        if (ForwardConnectList.TryGetValue(connectionId, out List<Guid> forwardConnectList))
        {
            return forwardConnectList;
        }
        return new List<Guid>();
    }
    
    /// <summary>
    /// 修改下一条地址
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="forwardConnectList"></param>
    public static async Task SetForwardListAsync(Guid connectionId, List<Guid> forwardConnectList)
    {
        ForwardConnectList.Remove(connectionId, out var _);
        ForwardConnectList.TryAdd(connectionId, forwardConnectList);
    }

    static EquipControlHelp()
    {
    }

    /// <summary>
    /// 获取当前点位的采集状态
    /// </summary>
    /// <param name="dataPointId"></param>
    /// <returns></returns>
    public static DataPointStatus GetStatusByPointId(Guid dataPointId)
    {
        return DataPointStatusDict.GetValueOrDefault(dataPointId, DataPointStatus.None);
    }

    /// <summary>
    /// 设置更新当前点位的采集状态
    /// </summary>
    /// <param name="dataPointId"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public static bool SetStatusByPointId(Guid dataPointId, DataPointStatus status)
    {
        if (!DataPointStatusDict.ContainsKey(dataPointId)) return DataPointStatusDict.TryAdd(dataPointId, status);
        DataPointStatusDict[dataPointId] = status;
        return true;
    }

    /// <summary>
    /// 修改连接参数
    /// </summary>
    /// <param name="connectId"></param>
    /// <param name="connectionString"></param>
    public static async Task UpdateEquipManagerAsync(Guid connectId, string connectionString)
    {
        //如果存在列表中，就修改参数
        EquipManagers.GetValueOrDefault(connectId)?.UpdateConnectionParameter(connectionString);
    }

    /// <summary>
    /// 增加一个采集连接
    /// </summary>
    /// <param name="connectId"></param>
    /// <param name="equipManager"></param>
    public static async Task AddDeviceManagerAsync(Guid connectId, IEquipManager equipManager)
    {
        EquipManagers.TryAdd(connectId, equipManager);
    }

    /// <summary>
    /// 获取一个采集连接
    /// </summary>
    /// <param name="connectId"></param>
    /// <returns></returns>
    public static async Task<IEquipManager?> GetDeviceManagerAsync(Guid connectId)
    {
        EquipManagers.TryGetValue(connectId, out var equipManager);
        return equipManager;
    }

    /// <summary>
    /// 删除一个采集连接
    /// </summary>
    /// <param name="connectId"></param>
    public static async Task RemoveDeviceManagerAsync(Guid connectId)
    {
        // EquipManagers.TryRemove(connectId, out _);
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    /// <param name="connectId"></param>
    public static async Task TestConnectionAsync(Guid connectId)
    {
        if (EquipManagers.TryGetValue(connectId, out var equipManager))
        {
            await equipManager.TestConnectionAsync();
        }
    }

    // /// <summary>
    // /// 给采集连接添加一个采集点位
    // /// </summary>
    // /// <param name="connectId"></param>
    // /// <param name="dataPoint"></param>
    // /// <returns></returns>
    // /// <exception cref="BusinessException"></exception>
    // public static async Task<bool> AddDevicePointAsync(Guid connectId, EquipDataPointAggregateRoot dataPoint)
    // {
    //     if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
    //     {
    //         return await deviceManager.AddDataPointAsync(dataPoint);
    //     }
    //
    //     throw new BusinessException("找不到该连接");
    // }

    // /// <summary>
    // /// 删除一个采集点位
    // /// </summary>
    // /// <param name="connectId"></param>
    // /// <param name="dataPoint"></param>
    // /// <returns></returns>
    // /// <exception cref="BusinessException"></exception>
    // public static async Task RemoveDevicePointAsync(Guid connectId, EquipDataPointAggregateRoot dataPoint)
    // {
    //     if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
    //     {
    //         await deviceManager.RemoveDataPointAsync(dataPoint);
    //         SetStatusByPointId(dataPoint.Id, DataPointStatus.None);
    //     }
    //     else
    //     {
    //         throw new ApplicationException("找不到该连接");
    //     }
    // }

    /// <summary>
    /// 判断是否是连接状态
    /// </summary>
    /// <param name="connectId"></param>
    /// <returns></returns>
    public static async Task<bool> IsConnectedAsync(Guid connectId)
    {
        if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
        {
            return await deviceManager.IsConnectedAsync();
        }

        return false;
    }

    /// <summary>
    /// 启动一个连接
    /// </summary>
    /// <param name="connectId"></param>
    /// <returns></returns>
    public static async Task StartDevice(Guid connectId)
    {
        if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
        {
            await deviceManager.StartAsync();
        }
    }

    /// <summary>
    /// 启动所有连接
    /// </summary>
    /// <returns></returns>
    public static async Task StartAll()
    {
        foreach (var manager in EquipManagers.Values)
        {
            await manager.StartAsync();
        }
    }

    /// <summary>
    /// 停止一个连接
    /// </summary>
    /// <param name="connectId"></param>
    /// <returns></returns>
    public static async Task StopDevice(Guid connectId)
    {
        if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
        {
            await deviceManager.DisConnectionAsync();
            await deviceManager.StopAsync();
        }
    }

    /// <summary>
    /// 停止所有连接
    /// </summary>
    /// <returns></returns>
    public static async Task StopAll()
    {
        foreach (var manager in EquipManagers.Values)
        {
            await manager.StopAsync();
        }
    }

    /// <summary>
    /// 开始采集
    /// </summary>
    /// <param name="connectId"></param>
    /// <param name="dataPointId"></param>
    public static async Task StartCollect(Guid connectId, Guid dataPointId)
    {
        if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
        {
            await deviceManager.StartAsync(dataPointId);
            SetStatusByPointId(dataPointId, DataPointStatus.Progress);
        }
    }

    /// <summary>
    /// 停止采集
    /// </summary>
    /// <param name="connectId"></param>
    /// <param name="dataPointId"></param>
    public static async Task StopCollect(Guid connectId, Guid dataPointId)
    {
        if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
        {
            await deviceManager.StopAsync(dataPointId);
            SetStatusByPointId(dataPointId, DataPointStatus.None);
        }
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="connectId"></param>
    /// <param name="buffer"></param>
    public static async Task SendMessageAsync(Guid connectId, byte[] buffer)
    {
        if (EquipManagers.TryGetValue(connectId, out IEquipManager deviceManager))
        {
            await deviceManager.IsConnectedAsync();
            await deviceManager.SendDataAsync(buffer);
        }
    }
}