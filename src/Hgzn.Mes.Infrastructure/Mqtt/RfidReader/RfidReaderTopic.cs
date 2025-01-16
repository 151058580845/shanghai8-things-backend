using Hgzn.Mes.Infrastructure.Mqtt.Topic;

namespace Hgzn.Mes.Infrastructure.Mqtt.RfidReader
{
    /// <summary>
    /// Rfid读写器消息组装
    /// 1.Rfid读写器设备连接
    /// 2.Rfid读写器设备断开
    /// 3.Rfid读写器设备重连
    /// 4.Rfid读写器设备数据读取数据传输（RfidReaderReceiveData）
    /// </summary>
    public class RfidReaderTopic : MqttTopic
    {
        //预定义
        public static string[] SubTopics { get; set; } = { "Equip/Up/RFIDReader/+/number1/state/+" };

        public TopicEnum? EquipType { get; set; } = TopicEnum.RfidReader;
        
        public string? EquipCode { get; set; }

        public static RfidReaderTopic FromIotString(string topic)
        {
            var nodes = topic.Split('/');

            var dir = nodes[1] switch
            {
                "up" => MqttDirection.Up,
                "down" => MqttDirection.Down,
                _ => throw new NotSupportedException(nodes[1])
            };
            var tag = nodes[5] switch
            {
                "state" => MqttTag.State,
                "data" => MqttTag.Data,
                "ota" => MqttTag.Ota,
                "cmd" => MqttTag.Cmd,
                "health" => MqttTag.Health,
                "calibration" => MqttTag.Calibration,
                _ => throw new NotSupportedException(nodes[6])
            };
            var res = new RfidReaderTopic
            {
                Prefix = nodes[0],
                Direction = dir,
                EquipType = Enum.Parse<TopicEnum>(nodes[2]),
                EquipCode = nodes[4],
                Tag = tag
            };
            return res;
        }

        public override string ToString()
        {
            var tag = Tag.ToString("F").ToLower();
            var dir = Direction.ToString("F").ToLower();
            if (EquipType is not null)
            {
                return $"{Prefix}/{dir}/{EquipType.ToString()}/+/{EquipCode}/+/{tag}";
            }
            else
            {
                return $"{Prefix}/{dir}/{tag}";
            }
        }
    }
}