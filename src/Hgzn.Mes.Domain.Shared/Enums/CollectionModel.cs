using System.ComponentModel;

namespace Hgzn.Mes.Domain.Shared.Enums;

public enum CollectionModel
{
}

/// <summary>
/// 传输模式
/// </summary>
public enum TransmissionMode
{
    [Description("实时传输")]
    RealTime,

    [Description("批量传输")]
    Batch,

    [Description("周期传输")]
    Periodic,
}

/// <summary>
/// 传输目的地
/// </summary>
public enum TranDestination
{
    InfluxDb,
    SqlDb
}

/// <summary>
/// 采集周期
/// </summary>
public enum CyclePeriodEnum
{
    Hour,
    Day,
    Week,
    Month,
    Quarter,
    Year,
    Custom
}

public enum CollectionStartMode
{
    [Description("手动开始")]
    Manual,

    [Description("自动")]
    Automatic,
}