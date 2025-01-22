using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Domain.ValueObjects.Message;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Message;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using Microsoft.Extensions.Logging;
using MQTTnet;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json.Nodes;

namespace Hgzn.Mes.Iot.Mqtt
{
    public class MqttMessageHandler
    {
        public MqttMessageHandler(
            IConnectionMultiplexer connectionMultiplexer,
            ILogger<MqttMessageHandler> logger)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }

        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<MqttMessageHandler> _logger;
        private IMqttExplorer _mqttExplorer = null!;
        private static ConcurrentDictionary<string, List<(int heart, int breath)>> _rawDataPackage = new();
        private const int countIndex = 60;

        public void Initialize(IMqttExplorer mqttExplorer)
        {
            _mqttExplorer = mqttExplorer;
        }

        public async Task HandleAsync(MqttApplicationMessage message)
        {
            var topic = IotTopic.FromIotString(message.Topic);
            if (topic.Direction == MqttDirection.Down) return;
            switch (topic.Tag)
            {
                case MqttTag.State:
                    var state = new DeviceStateMsg(message.ConvertPayloadToString());
                    await HandleStateAsync(topic, state);
                    break;

                case MqttTag.Data:
                    await HandleDataAsync(topic, message.PayloadSegment.Array!);
                    break;

                case MqttTag.Health:
                    //var health = new DeviceHealthMsg(message.ConvertPayloadToString());
                    //await HandleHealthAsync(topic, health, dbContext);
                    break;

                case MqttTag.Calibration:
                    await HandleCalibrationAsync(topic);
                    break;

                default:
                    return;
            }
        }

        private Task HandleStateAsync(IotTopic topic, DeviceStateMsg message)
        {
            throw new NotImplementedException();

        }

        private Task HandleDataAsync(IotTopic topic, byte[] msg)
        {
            throw new NotImplementedException();
        }

        private void HandleHealthAsync(IotTopic topic,
            DeviceHealthMsg message)
        {
            throw new NotImplementedException();
        }

        private async Task HandleCalibrationAsync(IotTopic topic)
        {
            var newTopic = topic;
            newTopic.Direction = MqttDirection.Down;
            await _mqttExplorer.PublishAsync(newTopic.ToString(), new DeviceCalibrationMsg().AsFrame());
            _logger.LogInformation($"device: {topic.DeviceUri} time calibrate succeed");
        }
    }
}