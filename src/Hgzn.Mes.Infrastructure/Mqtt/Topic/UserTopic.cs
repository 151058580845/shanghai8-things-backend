using Hgzn.Mes.Domain.Shared.Enums;

namespace Hgzn.Mes.Infrastructure.Mqtt.Topic
{
    public class UserTopic : MqttTopic
    {
        public string UserCode { get; set; } = null!;
        public const string UserCodeName = "user";

        public static UserTopic FromUserString(string topic)
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

            var res = new UserTopic
            {
                Direction = dir,
                UserCode = nodes[3],
                Tag = tag
            };
            return res;
        }

        public override string ToString()
        {
            var prefix = $"{Prefix:F}".ToLower();
            var tag = Tag.ToString("F").ToLower();
            var dir = Direction.ToString("F").ToLower();
            if (!string.IsNullOrEmpty(UserCode))
            {
                return $"{Prefix}/{dir}/{UserCodeName}/{UserCode}/{tag}";
            }
            else
            {
                return $"{prefix}/{dir}/{tag}";
            }
        }
    }
}