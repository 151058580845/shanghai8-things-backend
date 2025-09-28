using Hgzn.Mes.Domain.Entities.Base;
using Hgzn.Mes.Domain.Entities.Base.Audited;
using Hgzn.Mes.Domain.Shared;
using System.ComponentModel;
using SqlSugar;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;

/// <summary>
/// 氧浓度数据记录实体
/// </summary>
[SugarTable("oxygen_concentration_records")]
public class OxygenConcentrationRecord : UniversalEntity, ICreationAudited
{
    /// <summary>
    /// 房间号
    /// </summary>
    [Description("房间号")]
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// 氧浓度值（百分比）
    /// </summary>
    [Description("氧浓度值")]
    public float Concentration { get; set; }

    /// <summary>
    /// 记录时间
    /// </summary>
    [Description("记录时间")]
    public DateTime RecordTime { get; set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    [Description("设备ID")]
    public Guid DeviceId { get; set; }

    /// <summary>
    /// 本地IP地址
    /// </summary>
    [Description("本地IP地址")]
    public string LocalIp { get; set; } = string.Empty;

    /// <summary>
    /// 是否异常（超出正常范围）
    /// </summary>
    [Description("是否异常")]
    public bool IsAbnormal { get; set; }

    /// <summary>
    /// 异常类型（0=正常，1=过低，2=过高）
    /// </summary>
    [Description("异常类型")]
    public int AbnormalType { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Description("备注")]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Description("创建时间")]
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    [Description("创建人ID")]
    public Guid? CreatorId { get; set; }

    /// <summary>
    /// 创建人级别
    /// </summary>
    [Description("创建人级别")]
    public int CreatorLevel { get; set; }
}
