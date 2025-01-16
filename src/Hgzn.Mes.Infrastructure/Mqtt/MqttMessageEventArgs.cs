namespace Hgzn.Mes.Infrastructure.Mqtt;

public class MqttMessageEventArgs
{
    public string Topic { get; set; }
    public byte[] Payload { get; set; }

    // 构造函数
    public MqttMessageEventArgs(string topic, byte[] payload)
    {
        Topic = topic;
        Payload = payload;
    }
}