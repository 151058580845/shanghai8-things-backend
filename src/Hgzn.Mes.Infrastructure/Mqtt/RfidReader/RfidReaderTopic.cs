using Hgzn.Mes.Infrastructure.Mqtt.Topic;

namespace Hgzn.Mes.Infrastructure.Mqtt.RfidReader
{
    /// <summary>
    /// Rfid读写器消息组装
    /// 1.Rfid读写器设备连接
    /// 2.Rfid读写器设备断开
    /// 3.Rfid读写器设备重连
    /// 123使用
    /// 订阅方    "Equip/Down/RFIDReader/+/Cmd/+/"
    /// 发送方    "Equip/Up/RFIDReader/设备Id/Cmd/Connected/"
    /// 4.Rfid读写器设备事件(连接，断开，重连的事件信息)
    /// 订阅方    "Equip/Down/RFIDReader/+/state/+/"
    /// 发送方    "Equip/Up/RFIDReader/设备Id/state/Connected/"
    /// 5.Rfid读写器设备数据读取数据传输
    /// 订阅方    "Equip/Down/RFIDReader/+/Data/+/"
    /// 发送方    "Equip/Up/RFIDReader/设备Id/Data/+/"
    /// </summary>
    public class RfidReaderTopic : MqttTopic
    {
        //预定义
        public static string[] SubTopics { get; set; } =
        [
            "Equip/Up/RFIDReader/+/state/+/",
            $"{TopicTypeEnum.Equip.ToString()}/{MqttDirection.Up.ToString()}/{TopicEquipEnum.RfidReader.ToString()}/+/{MqttTag.State.ToString()}/{MqttState.Connected.ToString()}/"
        ];

        public TopicEquipEnum? EquipType { get; set; } = TopicEquipEnum.RfidReader;

        public string EquipId { get; set; } = "+";
        public string State { get; set; } = "+";

        public static RfidReaderTopic FromIotString(string topic)
        {
            var nodes = topic.Split('/');

            var dir = nodes[1] switch
            {
                "up" => MqttDirection.Up,
                "down" => MqttDirection.Down,
                _ => throw new NotSupportedException(nodes[1])
            };
            var tag = nodes[4] switch
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
                Prefix = Enum.Parse<TopicTypeEnum>(nodes[0]),
                Direction = dir,
                EquipType = Enum.Parse<TopicEquipEnum>(nodes[2]),
                EquipId = nodes[3],
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
                return $"{Prefix}/{dir}/{EquipType.ToString()}/{EquipId}/{tag}/{ State }";
            }
            else
            {
                return $"{Prefix}/{dir}/{tag}";
            }
        }
    }
}