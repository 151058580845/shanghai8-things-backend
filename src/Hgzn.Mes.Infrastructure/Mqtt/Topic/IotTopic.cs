

using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Infrastructure.Mqtt.Topic
{
    public class IotTopic : MqttTopic
    {
        // public string[] SubTopics { get; set; } = { $"iot/+/{DevTypeName}/+/{DevUriName}/+/+" };
        
        public string DeviceType { get; set; }
        public string IotId { get; set; }
        public string ProgramId { get; set; }
        
        public static IotTopic FromIotString(string topic)
        {
            var nodes = topic.Split('/');
            var prefix = nodes[0] switch
            {
                "iot" => TopicType.Iot,
                "app" => TopicType.App,
                _ => throw new NotSupportedException(nodes[0])
            };
            var dir = nodes[1] switch
            {
                "up" => MqttDirection.Up,
                "down" => MqttDirection.Down,
                _ => throw new NotSupportedException(nodes[1])
            };
            var tag = nodes[6] switch
            {
                "state" => MqttTag.State,
                "data" => MqttTag.Data,
                "ota" => MqttTag.Ota,
                "cmd" => MqttTag.Cmd,
                "health" => MqttTag.Health,
                "calibration" => MqttTag.Calibration,
                _ => throw new NotSupportedException(nodes[6])
            };

            var res = new IotTopic
            {
                Prefix = prefix,
                Direction = dir,
                DeviceType = nodes[2],
                IotId = nodes[3],
                ProgramId = nodes[4],
                Tag = tag
            };
            return res;
        }

        public override string ToString()
        {
            var pre = Prefix.ToString("F").ToLower();
            var tag = Tag.ToString("F").ToLower();
            var dir = Direction.ToString("F").ToLower();
            if (DeviceType is not null)
            {
                return $"{pre}/{dir}/{DeviceType}/{IotId}/{ProgramId}/{tag}/{State}";
            }
            else
            {
                return $"{pre}/{dir}/{tag}";
            }
        }
    }
}