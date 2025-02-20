

using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Infrastructure.Mqtt.Topic
{
    public class IotTopic : MqttTopic
    {
        public static string[] SubTopics { get; set; } = { $"iot/+/{EquipTypeName}/+/{ConnUriName}/+/+" };

        public const string EquipTypeName = "type";
        public const string ConnUriName = "uri";

        public EquipConnType ConnType { get; set; }

        public string? EquipType { get; set; }
        public string? ConnUri { get; set; }

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
                "alarm" => MqttTag.Alarm,
                "calibration" => MqttTag.Calibration,
                _ => throw new NotSupportedException(nodes[6])
            };
            Enum.TryParse<EquipConnType>(nodes[3], true, out var connType);
            var res = new IotTopic
            {
                Prefix = prefix,
                Direction = dir,
                EquipType = nodes[3],
                ConnType = connType,
                ConnUri = nodes[5],
                Tag = tag
            };
            return res;
        }

        public override string ToString()
        {
            var pre = Prefix.ToString("F");
            var tag = Tag.ToString("F");
            var dir = Direction.ToString("F");
            var result = ConnUri is not null ?
                $"{pre}/{dir}/{EquipTypeName}/{EquipType}/{ConnUriName}/{ConnUri}/{tag}" :
                $"{pre}/{dir}/{tag}";
            return result.ToLower();
        }
    }
}