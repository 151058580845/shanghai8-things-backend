using System.ComponentModel;

namespace Hgzn.Mes.Domain.Shared.Enums;

public enum EquipConnectEnum
{
    [Description("连接")]
    Connect = 1,
    [Description("断开连接")]
    DisConnect = 2,
    [Description("发生错误")]
    Fail = 3,
    [Description("取消连接")]
    CancelConnect = 4,
}