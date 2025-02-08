using Hgzn.Mes.Domain.ValueObjects.Message.Base;
using System.ComponentModel;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;

public class ModbusTcpConnInfo : IConnInfo
{
    /// <summary>
    /// IP地址
    /// </summary>
    [Description("Ip地址")]
    public string Address { get; set; } = null!;

    /// <summary>
    /// 端口号（默认502）
    /// </summary>
    [Description("端口号")] 
    public int Port { get; set; } = 502;

    /// <summary>
    /// 单元标识符（Unit Identifier）
    /// </summary>
    [Description("单元标识符")]
    public byte UnitIdentifier { get; set; } = 1;

    /// <summary>
    /// 接收时延
    /// </summary>
    [Description("接收时延")]
    public int ReceiveTimeout { get; set; } = 1000;

    /// <summary>
    /// 发送时延
    /// </summary>
    [Description("发送时延")] 
    public int SendTimeout { get; set; } = 1000;
    
    /// <summary>
    /// 首地址开始
    /// </summary>
    [Description("首地址开始")]
    public bool StartZero { get; set; } = true;
    
    /// <summary>
    /// 站点
    /// </summary>
    [Description("SlaveId")]
    public byte SlaveId { get; set; } = 1;

    /// <summary>
    /// 读取格式
    /// </summary>
    //[Description("Format")]
    //public DataOrderType DataType { get; set; } = DataOrderType.CDAB;
}