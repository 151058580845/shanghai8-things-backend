using Hgzn.Mes.Domain.Shared;

namespace Hgzn.Mes.Domain.Entities.Equip.EquipData;

/// <summary>
/// 氧浓度数据结构
/// </summary>
public class OxygenConcentrationData
{
    /// <summary>
    /// 房间号
    /// </summary>
    public string RoomNumber { get; set; } = string.Empty;

    /// <summary>
    /// 氧浓度值（百分比）
    /// </summary>
    public float Concentration { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    public Guid DeviceId { get; set; }

    /// <summary>
    /// 本地IP地址
    /// </summary>
    public string LocalIp { get; set; } = string.Empty;

    /// <summary>
    /// 是否异常（超出正常范围）
    /// </summary>
    public bool IsAbnormal { get; set; }

    /// <summary>
    /// 异常类型（0=正常，1=过低，2=过高）
    /// </summary>
    public int AbnormalType { get; set; }
}
