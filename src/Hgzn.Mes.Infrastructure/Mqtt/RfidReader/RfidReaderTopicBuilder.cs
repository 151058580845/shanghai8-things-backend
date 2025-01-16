using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Mysqlx.Crud;

namespace Hgzn.Mes.Infrastructure.Mqtt.RfidReader;

public class RfidReaderTopicBuilder: TopicBuilder
{
    private TopicEnum _deviceType;
    private string _deviceUri = null!;

    public override RfidReaderTopicBuilder WithPrefix(string prefix)
    {
        Prefix = prefix;
        return this;
    }

    public override RfidReaderTopicBuilder WithDirection(MqttDirection direction)
    {
        Direction = direction;
        return this;
    }

    public override RfidReaderTopicBuilder WithTag(MqttTag tag)
    {
        Tag = tag;
        return this;
    }

    public RfidReaderTopicBuilder WithDeviceType(TopicEnum equipType)
    {
        _deviceType = equipType;
        return this;
    }

    public RfidReaderTopicBuilder WithUri(string uri)
    {
        _deviceUri = uri;
        return this;
    }

    public override string Build() =>
        new RfidReaderTopic
        {
            Direction = MqttDirection.Down,
            Prefix = Prefix ?? "iot",
            Tag = Tag,
            EquipType = _deviceType,
            EquipCode = _deviceUri
        }.ToString();

    public static RfidReaderTopicBuilder CreateBuilder() => new();
}