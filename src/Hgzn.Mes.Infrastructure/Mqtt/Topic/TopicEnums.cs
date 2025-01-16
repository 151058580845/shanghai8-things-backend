using System.ComponentModel;

namespace Hgzn.Mes.Infrastructure.Mqtt.Topic;

public enum TopicEnum
{
    RfidReader,
    Rs232Reader
}

public enum MqttDirection: byte
{
    Up,
    Down,
}

public enum MqttTag
{
    [Description("设备状态")]
    State,

    [Description("数据传输")]
    Data,

    [Description("OTA更新")]
    Ota,

    [Description("命令控制")]
    Cmd,

    [Description("健康状态")]
    Health,

    [Description("校准数据")]
    Calibration,

    [Description("警报信息")]
    Alarm
}

public enum MqttState : byte
{
    [Description("连接")]
    Connected,

    [Description("停止")]
    Stop,

    [Description("重连")]
    Restart,

    [Description("遗愿消息")]
    Will
}