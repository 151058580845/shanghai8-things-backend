using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Infrastructure.Mqtt.Topic
{
    /// <summary>
    /// MqttTopic基础类
    /// </summary>
    public class MqttTopic
    {
        public TopicType Prefix { get; set; } = TopicType.Iot;
        public MqttDirection Direction { get; set; }
        public MqttTag Tag { get; set; }

        public static MqttTopic FromString(string topic)
        {
            var nodes = topic.Split('/');
            var dir = nodes[1] switch
            {
                "up" => MqttDirection.Up,
                "down" => MqttDirection.Down,
                _ => throw new NotSupportedException()
            };
            var tag = nodes[^1] switch
            {
                "state" => MqttTag.State,
                "data" => MqttTag.Data,
                "ota" => MqttTag.Ota,
                "cmd" => MqttTag.Cmd,
                "health" => MqttTag.Health,
                "calibration" => MqttTag.Calibration,
                _ => throw new NotSupportedException()
            };

            var res = new MqttTopic
            {
                Direction = dir,
                Tag = tag
            };
            return res;
        }

        public override string ToString()
        {
            var tag = Tag!.ToString("F");
            var dir = Direction.ToString("F");
            return $"{Prefix}/{dir}/{tag}".ToLower();
        }
    }
    
}