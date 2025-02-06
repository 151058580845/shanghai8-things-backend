using System.ComponentModel;

namespace Hgzn.Mes.Iot.ProtocolManager.Protocols;

public class ProtocolRfidClient
{
    /// <summary>
    /// IP地址
    /// </summary>
    [Description("Ip地址")]
    public string Address { get; set; }

    /// <summary>
    /// 端口号（默认44818）
    /// </summary>
    [Description("Port")]
    public int Port { get; set; } = 44818;

    /// <summary>
    /// 会话超时时间（Session Timeout）
    /// </summary>
    [Description("SessionTimeout")]
    public int SessionTimeout { get; set; } = 1000;

    /// <summary>
    /// 注册会话（Register Session）
    /// </summary>
    [Description("RegisterSession")]
    public bool RegisterSession { get; set; } = true;
}