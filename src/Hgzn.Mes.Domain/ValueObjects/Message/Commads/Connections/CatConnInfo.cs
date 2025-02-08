using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;

public class CatConnInfo : IConnInfo
{
    /// <summary>
    /// 主站接口名称（Network Interface Name）
    /// </summary>
    public string MasterInterfaceName { get; set; } = null!;

    /// <summary>
    /// 同步模式（Sync Mode）
    /// </summary>
    public string SyncMode { get; set; } = null!;

    /// <summary>
    /// 周期时间（Cycle Time），单位：微秒
    /// </summary>
    public int CycleTime { get; set; }

    /// <summary>
    /// 从站配置文件路径（Slave Configuration File Path）
    /// </summary>
    public string SlaveConfigFilePath { get; set; } = null!;

}