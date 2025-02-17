using System.ComponentModel;

namespace Hgzn.Mes.Domain.Shared.Enums;

/// <summary>
/// 定义哪些设备可以进行Iot操作
/// </summary>
public enum EquipConnType
{
    /// <summary>
    /// Rfid读写器
    /// </summary>
    [Description("Rfid读写器")]
    RfidReader = 1,
    /// <summary>
    /// 数据采集适配器
    /// </summary>
    [Description("数据采集适配器")]
    IotServer,
}