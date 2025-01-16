namespace Hgzn.Mes.Infrastructure.Mqtt;

public class MqttSettings
{
    public string ClientId { get; set; } = "DotNetCoreClient";
    public string Broker { get; set; } = "localhost";
    public int Port { get; set; } = 1883;
    public bool UseTls { get; set; } = false;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public List<TopicSubscription> Topics { get; set; } = [];
    public int ReconnectDelaySeconds { get; set; } = 5;
}

public class TopicSubscription
{
    public string Topic { get; set; } = null!;
    public int QoS { get; set; } = 0; // 0: AtMostOnce, 1: AtLeastOnce, 2: ExactlyOnce
}