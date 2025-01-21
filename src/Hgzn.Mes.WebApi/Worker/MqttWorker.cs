using Hgzn.Mes.Infrastructure.Mqtt.Manager;

namespace Hgzn.Mes.WebApi.Worker
{
    public class MqttWorker : BackgroundService
    {
        private readonly ILogger<MqttWorker> _logger;
        private readonly IMqttExplorer _mqttExplorer;

        public MqttWorker(
            ILogger<MqttWorker> logger,
            IMqttExplorer mqttExplorer)
        {
            _logger = logger;
            _mqttExplorer = mqttExplorer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _mqttExplorer.StartAsync();
                _logger.LogInformation("mqtt worker is connected!");
            }
            catch
            {
                _logger.LogError("mqtt worker is restarting");
                await _mqttExplorer.RestartAsync();
            }
        }
    }
}
