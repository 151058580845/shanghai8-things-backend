using System.ComponentModel;

namespace Hgzn.Mes.Domain.Shared.Enums;

public enum Equip
{
}

// /// <summary>
// /// 设备类型
// /// </summary>
// public enum EquipType
// {
//     [Description("用户设备")]
//     UserEquip=1,
//     [Description("RFID读写器")]  //只给TcpClient选项配置
//     RfidReader=2,
//     [Description("上位机设备")]
//     UpperCompute=3
// }

/// <summary>
/// 设备重要度
/// </summary>
public enum EquipLevelEnum
{
    [Description("关键设备")] Important = 1,

    [Description("一般设备")] General = 2,

    [Description("普通设备")] Basic = 3
}

/// <summary>
/// 状态数据存redis
/// 在上传数据则为运行状态
/// 给了状态，是连接状态，但是状态为不采集数据，则为暂停状态
/// 未连接则为停止状态
/// </summary>
public enum EquipOperationStatus
{
    Running, //运行
    Paused, //暂停
    Stopped //停止
}

/// <summary>
/// 状态数据存redis
/// 传上来的数据一直没有错误是健康
/// 有少数的几个错误数据，亚健康
/// 很多错误，故障  这个后期让客户定
/// </summary>
public enum EquipHealthStatus
{
    Health, //健康
    SubHealth, //亚健康
    Fault //故障
}
/// <summary>
/// 设备当前是否跟丢
/// </summary>
public enum DeviceStatus
{
    Normal, // 正常
    Lost, // 丢失
}