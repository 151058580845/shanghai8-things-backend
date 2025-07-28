using Hgzn.Mes.Domain.ValueObjects.Message.Base;

namespace Hgzn.Mes.Domain.ValueObjects.Message.Commads.Connections;

public class ModbusRtuConnInfo : IConnInfo
{

    /// <summary>
    /// 波特率（Baud Rate）
    /// </summary>
    public string BaudRate { get; set; }

    /// <summary>
    /// 数据位（Data Bits）
    /// </summary>
    public int DataBits { get; set; }

    /// <summary>
    /// 奇偶校验（Parity）
    /// </summary>
    public string Parity { get; set; }

    /// <summary>
    /// 停止位（Stop Bits）
    /// </summary>
    public string StopBits { get; set; }

    /// <summary>
    /// 从站地址（Slave Address）
    /// </summary>
    public byte slaveAddress { get; set; }

    /// <summary>
    /// 串行端口名称（如COM1, COM2）
    /// </summary>
    public int PortName { get; set; }
    
    /// <summary>
    /// 读取格式
    /// </summary>
    //[Description("Format")]
    //public DataOrderType DataType { get; set; } = DataOrderType.CDAB;

    public int ReadTimeout { get; set; } = 1000;
    public int WriteTimeout { get; set; } = 1000;

}