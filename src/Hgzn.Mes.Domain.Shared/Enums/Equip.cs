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
    [Description("关键设备")]
    Important=1,
    [Description("一般设备")]
    General=2,
    [Description("普通设备")]
    Basic=3
}

public enum EquipOperationStatus
{
    Running,//运行
    Standby,//待机
    Paused,//暂停
    Stopped //停止
}