using Hgzn.Mes.Infrastructure.Mqtt.Topic;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager
{
    public class TopicBuilder
    {
        protected string? Prefix ;
        protected MqttDirection Direction;
        protected MqttTag Tag;

        public TopicBuilder()
        { }

        public virtual TopicBuilder WithPrefix(string prefix)
        {
            Prefix = prefix;
            return this;
        }

        public virtual TopicBuilder WithDirection(MqttDirection direction)
        {
            Direction = direction;
            return this;
        }

        public virtual TopicBuilder WithTag(MqttTag tag)
        {
            Tag = tag;
            return this;
        }

        public virtual string Build() =>
            new MqttTopic { Direction = Direction, Prefix = Prefix ?? throw new ArgumentNullException(), Tag = Tag }.ToString();

    }
}