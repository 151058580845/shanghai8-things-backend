using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using Hgzn.Mes.Infrastructure.Mqtt.Topic;
using System;

namespace Hgzn.Mes.WebApi.Worker
{
    public class EquipAlarmWorker : BackgroundService
    {
        public EquipAlarmWorker(
            ILogger<EquipAlarmWorker> logger,
            IMqttExplorer mqttExplorer,
            IEquipLedgerService ledgerService)
        {
            _logger = logger;
            _mqttExplorer = mqttExplorer;
            _ledgerService = ledgerService;
        }

        private readonly ILogger<EquipAlarmWorker> _logger;
        private readonly IMqttExplorer _mqttExplorer;
        private readonly IEquipLedgerService _ledgerService;

        protected override async Task ExecuteAsync(CancellationToken _)
        {
            await Task.Delay(1 * 60 * 1000);
            while (true)
            {
                var equips = await _ledgerService.GetMissingDevicesAlarmAsync();
                var tasks = equips.Select(async e =>
                {
                    var topic = UserTopicBuilder
                        .CreateUserBuilder()
                        .WithPrefix(TopicType.App)
                        .WithDirection(MqttDirection.Up)
                        .WithTag(MqttTag.Alarm)
                        .WithUri(e.EquipCode!)
                        .Build();
                    
                    // 使用支持断点续传的发布方法，告警消息优先级较高
                    if (_mqttExplorer is Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport.IMqttExplorerWithOffline mqttWithOffline)
                    {
                        await mqttWithOffline.PublishWithOfflineSupportAsync(topic, [], priority: 1, maxRetryCount: 5);
                    }
                    else
                    {
                        await _mqttExplorer.PublishAsync(topic, []);
                    }
                });
                await Task.WhenAll(tasks);
                await Task.Delay(15 * 60 * 1000);
            }
        }
    }
}
