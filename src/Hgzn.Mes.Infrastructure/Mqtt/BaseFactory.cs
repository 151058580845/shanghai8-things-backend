using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Message.Base;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;

namespace Hgzn.Mes.Infrastructure.Mqtt;

public abstract class BaseFactory
{
    public IIotMessage _jsonMessage;
    public MqttTopic _topic;
    public TopicBuilder _topicBuilder;

    public abstract IIotMessage GetIotMessage(MqttState state);
    public abstract MqttTopic GetTopic();
    
    public abstract string GetTopicStr();
}