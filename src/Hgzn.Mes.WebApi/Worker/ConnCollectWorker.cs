using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Entities.Equip.EquipControl;
using Hgzn.Mes.Domain.Entities.Equip.EquipManager;
using Hgzn.Mes.Domain.Shared;
using Hgzn.Mes.Domain.Shared.Enums;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using SqlSugar;
using StackExchange.Redis;

namespace Hgzn.Mes.WebApi.Worker
{
    public class ConnCollectWorker : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly ISqlSugarClient _sqlClient;
        private readonly IConfiguration _configuration;
        private readonly IMqttExplorer _mqttExplorer;
        private readonly ISqlSugarClient _dbContext;
        private readonly IEquipConnService _equipConnectService;
        private readonly IDatabase _redis;
        public ConnCollectWorker(
            ISqlSugarClient sqlClient,
            IConfiguration configuration,
            IMqttExplorer mqttExplorer,
            ISqlSugarClient dbContext,
            IConnectionMultiplexer connectionMultiplexer,
            IEquipConnService equipConnectService)
        {
            _sqlClient = sqlClient;
            _configuration = configuration;
            _mqttExplorer = mqttExplorer;
            _dbContext = dbContext;
            _equipConnectService = equipConnectService;
            _redis = connectionMultiplexer.GetDatabase();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                //延迟启动等待数据库初始化
                await Task.Delay(3000, stoppingToken);
                var interval = _configuration.GetValue<int>("ReConnInterval");
                while (!stoppingToken.IsCancellationRequested)
                {
                    LoggerAdapter.LogDebug($"AG - 检测是否有设备需要连接...");
                    EquipConnect[] connections = await _sqlClient.Queryable<EquipConnect>()
                        .Where(ec => ec.State && ec.ConnectStr != null)
                        .Where(ec => ec.EquipLedger!.EquipType!.Id == EquipType.RKType.Id ||
                        ec.EquipLedger!.EquipType!.Id == EquipType.IotType.Id)
                        .Where(ec => !ec.SoftDeleted)
                        .ToArrayAsync();
                    foreach (EquipConnect? connection in connections)
                    {
                        LoggerAdapter.LogDebug($"AG - 准备连接:{connection.Id}");
                        string key = string.Format(CacheKeyFormatter.EquipState, connection.EquipId, connection.Id);
                        if (_redis.StringGet(key) != 3)
                        {
                            if (!connection.ConnectState)
                            {
                                await _equipConnectService.PutStartConnect(connection.Id);
                                await Task.Delay(500, stoppingToken);
                            }
                        }

                    }
                    await Task.Delay(1000 * interval, stoppingToken);
                }
            }
            catch (Exception e)
            {
                LoggerAdapter.LogError(e.Message);
            }
        }
    }
}
