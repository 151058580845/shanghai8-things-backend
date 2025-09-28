using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT离线消息数据库迁移服务
    /// 在应用启动时自动执行数据库迁移
    /// </summary>
    public class MqttOfflineMigrationService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MqttOfflineMigrationService> _logger;

        public MqttOfflineMigrationService(IServiceProvider serviceProvider, ILogger<MqttOfflineMigrationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("启动MQTT离线消息数据库迁移服务...");

                using var scope = _serviceProvider.CreateScope();
                var sqlSugarClient = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
                var migration = new MqttOfflineDatabaseMigration(sqlSugarClient, 
                    scope.ServiceProvider.GetRequiredService<ILogger<MqttOfflineDatabaseMigration>>());

                // 检查是否已初始化
                var isInitialized = await migration.IsInitializedAsync();
                
                if (!isInitialized)
                {
                    _logger.LogInformation("检测到数据库未初始化，开始执行迁移...");
                    await migration.MigrateAsync();
                    _logger.LogInformation("数据库迁移完成");
                }
                else
                {
                    _logger.LogInformation("数据库已初始化，跳过迁移");
                }

                // 验证数据库结构
                var isValid = await migration.ValidateDatabaseStructureAsync();
                if (!isValid)
                {
                    _logger.LogWarning("数据库结构验证失败，可能需要重新迁移");
                }

                _logger.LogInformation("MQTT离线消息数据库迁移服务启动完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MQTT离线消息数据库迁移服务启动失败");
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("MQTT离线消息数据库迁移服务已停止");
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// MQTT离线消息数据库迁移服务扩展
    /// </summary>
    public static class MqttOfflineMigrationServiceExtensions
    {
        /// <summary>
        /// 注册MQTT离线消息数据库迁移服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMqttOfflineMigration(this IServiceCollection services)
        {
            services.AddHostedService<MqttOfflineMigrationService>();
            return services;
        }

        /// <summary>
        /// 手动执行数据库迁移
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task MigrateMqttOfflineDatabaseAsync(this IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<MqttOfflineDatabaseMigration>>();
            var sqlSugarClient = serviceProvider.GetRequiredService<ISqlSugarClient>();
            
            var migration = new MqttOfflineDatabaseMigration(sqlSugarClient, logger);
            await migration.MigrateAsync();
        }

        /// <summary>
        /// 验证数据库结构
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task<bool> ValidateMqttOfflineDatabaseAsync(this IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<MqttOfflineDatabaseMigration>>();
            var sqlSugarClient = serviceProvider.GetRequiredService<ISqlSugarClient>();
            
            var migration = new MqttOfflineDatabaseMigration(sqlSugarClient, logger);
            return await migration.ValidateDatabaseStructureAsync();
        }

        /// <summary>
        /// 回滚数据库迁移
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task RollbackMqttOfflineDatabaseAsync(this IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<MqttOfflineDatabaseMigration>>();
            var sqlSugarClient = serviceProvider.GetRequiredService<ISqlSugarClient>();
            
            var migration = new MqttOfflineDatabaseMigration(sqlSugarClient, logger);
            await migration.RollbackAsync();
        }
    }
}
