using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;

namespace Hgzn.Mes.Infrastructure.Mqtt.RfidReader;

public class RfidReaderTopicBuilder: TopicBuilder
{
    private TopicEquipEnum _deviceType;
    private string _equipId = null!;
    private MqttState? _state;

    public override RfidReaderTopicBuilder WithPrefix(TopicTypeEnum prefix)
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

    public RfidReaderTopicBuilder WithDeviceType(TopicEquipEnum equipType)
    {
        _deviceType = equipType;
        return this;
    }

    public RfidReaderTopicBuilder WithEquipId(string equipId)
    {
        _equipId = equipId;
        return this;
    }

    public RfidReaderTopicBuilder WithState(MqttState state)
    {
        _state = state;
        return this;
    }

    public override string Build() =>
        new RfidReaderTopic
        {
            Direction = MqttDirection.Down,
            Prefix = Prefix ?? TopicTypeEnum.Equip,
            Tag = Tag,
            EquipType = _deviceType,
            EquipId = _equipId,
            State = (_state==null?"+":_state.ToString()) ?? "+"
        }.ToString();

    public static RfidReaderTopicBuilder CreateBuilder() => new();
}