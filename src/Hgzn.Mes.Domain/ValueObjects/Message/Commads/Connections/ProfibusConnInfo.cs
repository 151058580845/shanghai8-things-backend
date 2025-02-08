using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;

public class ProfibusConnInfo : IConnInfo
{
    /// <summary>
    /// 设备地址（Station Address）
    /// </summary>
    public int StationAddress { get; set; }

    /// <summary>
    /// 波特率（Baud Rate）
    /// </summary>
    public int BaudRate { get; set; }

    /// <summary>
    /// 总线参数配置文件路径
    /// </summary>
    public string BusParameterConfigPath { get; set; } = null!;

}