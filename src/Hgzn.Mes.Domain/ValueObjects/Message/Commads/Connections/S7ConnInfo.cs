using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;

public class S7ConnInfo : IConnInfo
{
    /// <summary>
    /// PLC的IP地址
    /// </summary>
    public string IpAddress { get; set; } = null!;

    /// <summary>
    /// Rack编号
    /// </summary>
    public int Rack { get; set; }

    /// <summary>
    /// Slot编号
    /// </summary>
    public int Slot { get; set; }

    /// <summary>
    /// CPU类型
    /// </summary>
    public string CpuType { get; set; } = null!;

    /// <summary>
    /// 连接超时时间（单位：毫秒）
    /// </summary>
    public int ConnectionTimeout { get; set; }

}