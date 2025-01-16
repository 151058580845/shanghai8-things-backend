using Hgzn.Mes.Infrastructure.Mqtt.Message.Base;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;

namespace Hgzn.Mes.Infrastructure.Mqtt.RfidReader;

public class RfidReaderFactory : BaseFactory
{
    public string Protocol { get; set; }
    public string Code { get; set; }

    public RfidReaderFactory(string protocol, string code, MqttTopic topic)
    {
        _jsonMessage = new RfidReaderStateMsg();
        _topic = topic;
        _topicBuilder = new RfidReaderTopicBuilder();
        this.Protocol = protocol;
        this.Code = code;
    }

    public override IIotMessage GetIotMessage(MqttState state)
    {
        return new RfidReaderStateMsg()
        {
            Protocol = Protocol,
            EquipCode = Code,
            TimeStamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            mqttState = state
        };
    }

    public override MqttTopic GetTopic()
    {
        return _topic;
    }

    public override string GetTopicStr()
    {
        return _topic.ToString();
    }
}