using System.ComponentModel;
using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipManager;

/// <summary>
/// 设备每日运行时长记录
/// </summary>
[Description("设备每日运行时长")]
public class EquipDailyRuntime : UniversalEntity, ICreationAudited
{
    /// <summary>
    /// 设备ID
    /// </summary>
    [Description("设备ID")]
    public Guid EquipId { get; set; }

    /// <summary>
    /// 系统编号
    /// </summary>
    [Description("仿真试验系统识别编码")]
    public byte SystemNumber { get; set; }

    /// <summary>
    /// 设备类型编号
    /// </summary>
    [Description("设备类型识别编码")]
    public byte DeviceTypeNumber { get; set; }

    /// <summary>
    /// 记录日期
    /// </summary>
    [Description("记录日期")]
    public DateTime RecordDate { get; set; }

    /// <summary>
    /// 当日运行时长（秒）
    /// </summary>
    [Description("当日运行时长（秒）")]
    public uint RunningSeconds { get; set; }

    /// <summary>
    /// 当日运行时长（小时，保留2位小数）
    /// </summary>
    [Description("当日运行时长（小时）")]
    public decimal RunningHours => Math.Round((decimal)RunningSeconds / 3600, 2);

    /// <summary>
    /// 数据来源（Redis/Manual/Import等）
    /// </summary>
    [Description("数据来源")]
    public string DataSource { get; set; } = "Redis";

    /// <summary>
    /// 备注
    /// </summary>
    [Description("备注")]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 创建者ID
    /// </summary>
    public Guid? CreatorId { get; set; }

    /// <summary>
    /// 创建者级别
    /// </summary>
    public int CreatorLevel { get; set; } = 0;
}
