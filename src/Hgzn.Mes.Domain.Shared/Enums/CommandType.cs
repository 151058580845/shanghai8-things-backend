
using System.ComponentModel;

namespace Hgzn.Mes.Domain.Shared.Enums
{
    public enum CmdType
    {
        Conn,
    }

    public enum ConnStateType
    {
        On = 1,
        Off,
        Run,
        Stop
    }

    public enum EquipNoticeType
    {
        [Description("设备上线")] Online,
        [Description("设备下线")] Offline,
        [Description("告警")] Alarm
    }

    /// <summary>
    /// 通信协议枚举类型
    /// </summary>
    public enum ConnType
    {
        /// <summary>
        /// Modbus RTU 协议
        /// </summary>
        [Description("Modbus RTU")]
        ModbusRtu = 1,

        /// <summary>
        /// Modbus ASCII 协议
        /// </summary>
        [Description("Modbus ASCII")]
        ModbusAscii = 2,

        /// <summary>
        /// Modbus TCP 协议
        /// </summary>
        [Description("Modbus TCP")]
        ModbusTcp = 3,

        /// <summary>
        /// Modbus UDP 协议
        /// </summary>
        [Description("Modbus UDP")]
        ModbusUdp = 4,

        /// <summary>
        /// HTTP 协议
        /// </summary>
        [Description("HTTP")]
        Http = 5,

        /// <summary>
        /// Socket 协议
        /// </summary>
        [Description("Socket")]
        Socket = 6,

        /// <summary>
        /// 串口 协议
        /// </summary>
        [Description("SerialPort")]
        SerialPort = 7,

        /// <summary>
        /// TcpServer
        /// </summary>
        [Description("TcpServer")]
        TcpServer = 8,

    }
}
