using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;

public class ProfinetConnInfo : IConnInfo
{
    /// <summary>
    /// 设备名称（Device Name）
    /// </summary>
    public string DeviceName { get; set; } = null!;

    /// <summary>
    /// IP地址
    /// </summary>
    public string IpAddress { get; set; } = null!;

    /// <summary>
    /// 子网掩码
    /// </summary>
    public string SubnetMask { get; set; } = null!;

    /// <summary>
    /// 网关
    /// </summary>
    public string Gateway { get; set; } = null!;

    /// <summary>
    /// 实时等级（Real-Time Class）
    /// </summary>
    public string RealTimeClass { get; set; } = null!;

}