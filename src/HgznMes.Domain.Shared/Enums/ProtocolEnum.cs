using System.ComponentModel;

namespace HgznMes.Domain.Shared.Enums;

/// <summary>
/// 通信协议枚举类型
/// </summary>
public enum ProtocolEnum
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
    /// MQTT 协议
    /// </summary>
    [Description("MQTT")]
    Mqtt = 6,
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
    /// <summary>
    /// TcpClient
    /// </summary>
    [Description("TcpClient")]
    TcpClient = 9,
    /// <summary>
    /// RfidReaderClient
    /// </summary>
    [Description("RfidReaderClient")]
    RfidReaderClient = 10,
    // /// <summary>
    // /// OPC UA 协议
    // /// </summary>
    // [Description("OPCUA")]
    // OPCUA = 2,
    //
    // /// <summary>
    // /// OPC DA 协议
    // /// </summary>
    // [Description("OPCDA")]
    // OPCDA = 3,
    //

    //
    // /// <summary>
    // /// MQTT-SN 协议
    // /// </summary>
    // [Description("MQTTSN")]
    // MQTTSN = 5,
    //
    // /// <summary>
    // /// EtherNet/IP 协议
    // /// </summary>
    // [Description("EtherNetIP")]
    // EtherNetIP = 6,
    //
    // /// <summary>
    // /// EtherCAT 协议
    // /// </summary>
    // [Description("EtherCAT")]
    // EtherCAT = 7,
    //
    // /// <summary>
    // /// Profinet 协议
    // /// </summary>
    // [Description("Profinet")]
    // Profinet = 8,
    //
    // /// <summary>
    // /// Profibus 协议
    // /// </summary>
    // [Description("Profibus")]
    // Profibus = 9,
    //
    // /// <summary>
    // /// CANopen 协议
    // /// </summary>
    // [Description("CANopen")]
    // CANopen = 10,
    //
    // /// <summary>
    // /// DeviceNet 协议
    // /// </summary>
    // [Description("DeviceNet")]
    // DeviceNet = 11,
    //
    // /// <summary>
    // /// BACnet 协议
    // /// </summary>
    // [Description("BACnet")]
    // BACnet = 12,
    //
    // /// <summary>
    // /// DNP3 协议
    // /// </summary>
    // [Description("DNP3")]
    // DNP3 = 13,
    //
    // /// <summary>
    // /// HART 协议
    // /// </summary>
    // [Description("HART")] 
    // HART = 14,
    //
    // /// <summary>
    // /// S7 协议（西门子 PLC）
    // /// </summary>
    // [Description("S7")]
    // S7 = 15,
    //
    // /// <summary>
    // /// HTTP/HTTPS 协议
    // /// </summary>
    // [Description("HTTP")]
    // HTTP = 16,
    //
    // /// <summary>
    // /// FTP/SFTP 协议
    // /// </summary>
    // [Description("FTP")] 
    // FTP = 17,
    //
    // /// <summary>
    // /// SNMP 协议
    // /// </summary>
    // [Description("SNMP")]
    // SNMP = 18,
    //
    // /// <summary>
    // /// SOAP 协议
    // /// </summary>
    // [Description("SOAP")]
    // SOAP = 19
}
