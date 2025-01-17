namespace Hgzn.Mes.Infrastructure.Mqtt.RfidReader;

public class RfidReaderReceiveData
{
    /// <summary>
    /// 读写器编号
    /// </summary>
    public string SerialNumber { get; set; } = null!;
    /// <summary>
    /// 设备主键
    /// </summary>
    public string EquipId { get; set; } = null!;
    /// <summary>
    /// 设备资产编号
    /// </summary>
    public string EquipAssetNumber { get; set; } = null!;
    /// <summary>
    /// 标签编号
    /// </summary>
    public string Tid { get; set; } = null!;
}
