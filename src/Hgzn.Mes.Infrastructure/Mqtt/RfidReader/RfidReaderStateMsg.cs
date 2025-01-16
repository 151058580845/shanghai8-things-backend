using System.Text.Json.Nodes;
using Hgzn.Mes.Infrastructure.Mqtt.Message.Base;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;

namespace Hgzn.Mes.Infrastructure.Mqtt.RfidReader;

public class RfidReaderStateMsg : IJsonMessage
{
    /// <summary>
    /// 设备协议
    /// </summary>
    public string Protocol { get; set; }
    /// <summary>
    /// 设备编号
    /// </summary>
    public string EquipCode { get; set; }
    /// <summary>
    /// 时间戳
    /// </summary>
    public long TimeStamp { get; set; }
    /// <summary>
    /// 要做的事情
    /// </summary>
    public MqttState mqttState { get; set; }
    
    public JsonNode? Node { get; set; }
}