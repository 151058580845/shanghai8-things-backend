using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Hgzn.Mes.Infrastructure.Mqtt.Manager.OfflineSupport
{
    /// <summary>
    /// MQTT服务注册扩展方法
    /// </summary>
    public static class MqttServiceExtensions
    {
        /// <summary>
        /// 注册支持断点续传的MQTT服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddMqttWithOfflineSupport(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册Redis连接
            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddSingleton<IConnectionMultiplexer>(provider =>
                    ConnectionMultiplexer.Connect(redisConnectionString));
            }

            // 注册离线存储（优先使用混合存储）
            services.AddScoped<IMqttOfflineStorage, MqttOfflineHybridStorage>();
            services.AddScoped<IMqttOfflineDatabaseStorage, MqttOfflineDatabaseStorage>();

            // 注册支持断点续传的MQTT管理器
            services.AddScoped<IMqttExplorerWithOffline, MqttExplorerWithOffline>();
            
            // 为了向后兼容，也注册原来的接口
            services.AddScoped<IMqttExplorer>(provider => 
                provider.GetRequiredService<IMqttExplorerWithOffline>());

            // 注册数据库迁移服务
            services.AddMqttOfflineMigration();

            return services;
        }

        /// <summary>
        /// 注册支持断点续传的MQTT服务（使用数据库存储）
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddMqttWithDatabaseStorage(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册Redis连接（可选）
            var redisConnectionString = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnectionString))
            {
                services.AddSingleton<IConnectionMultiplexer>(provider =>
                    ConnectionMultiplexer.Connect(redisConnectionString));
            }

            // 注册数据库存储
            services.AddScoped<IMqttOfflineStorage, MqttOfflineDatabaseStorage>();
            services.AddScoped<IMqttOfflineDatabaseStorage, MqttOfflineDatabaseStorage>();

            // 注册支持断点续传的MQTT管理器
            services.AddScoped<IMqttExplorerWithOffline, MqttExplorerWithOffline>();
            
            // 为了向后兼容，也注册原来的接口
            services.AddScoped<IMqttExplorer>(provider => 
                provider.GetRequiredService<IMqttExplorerWithOffline>());

            // 注册数据库迁移服务
            services.AddMqttOfflineMigration();

            return services;
        }

        /// <summary>
        /// 注册标准的MQTT服务（不包含断点续传功能）
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddStandardMqtt(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册标准的MQTT管理器
            services.AddScoped<IMqttExplorer, ApiMqttExplorer>();
            
            return services;
        }
    }
}
