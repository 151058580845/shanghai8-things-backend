using System;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT离线消息实现
    /// </summary>
    public class MqttOfflineMessage : IMqttOfflineMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Topic { get; set; } = string.Empty;
        public byte[] Payload { get; set; } = Array.Empty<byte>();
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public int RetryCount { get; set; } = 0;
        public int MaxRetryCount { get; set; } = 3;
        public int Priority { get; set; } = 0;
        public bool IsSent { get; set; } = false;
        public DateTime? SentTime { get; set; }

        public MqttOfflineMessage()
        {
        }

        public MqttOfflineMessage(string topic, byte[] payload, int priority = 0, int maxRetryCount = 3)
        {
            Topic = topic;
            Payload = payload;
            Priority = priority;
            MaxRetryCount = maxRetryCount;
        }
    }
}
