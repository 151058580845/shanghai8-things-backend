using Autofac;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Mqtt;
using Hgzn.Mes.Infrastructure.Mqtt.Manager;
using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using RfidReaderClient.mqtt;
using SqlSugar;
using Module = Autofac.Module;

namespace RfidReaderClient.utils.Autofac;

public class SingletonModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        #region 注册sqlSugar

        builder.Register(context =>
            {
                var setting = context.Resolve<IConfiguration>().GetSection(nameof(DbConnOptions))
                    .Get<DbConnOptions>() ?? throw new Exception("sqlsugar config not found!");
                return new SqlSugarClient(SqlSugarContext.Build(setting));
            })
            .As<ISqlSugarClient>()
            .SingleInstance();
        builder.Register(context =>
            {
                var setting = context.Resolve<IConfiguration>().GetSection(nameof(DbConnOptions))
                    .Get<DbConnOptions>() ?? throw new Exception("sqlsugar config not found!");
                var logger = context.Resolve<ILogger<SqlSugarContext>>();
                var client = context.Resolve<ISqlSugarClient>();
                return new SqlSugarContext(logger, setting, client);
            })
            .PropertiesAutowired()
            .SingleInstance();

        #endregion
        
        #region 注册Mqtt

        builder.Register(context =>
            {
                var setting = context.Resolve<IConfiguration>().GetSection(nameof(MqttSettings))
                    .Get<MqttSettings>() ?? throw new Exception("Mqtt config not found!");
                return setting;
            })
            .As<MqttSettings>()
            .SingleInstance();
        builder.Register(context =>
            {
                var factory = new MqttFactory();
                return factory.CreateManagedMqttClient();
            })
            .As<IManagedMqttClient>()
            .SingleInstance();
        builder.Register(context =>
            {
                var logger = context.Resolve<ILogger<ApiMqttPub>>();
                var client = context.Resolve<IManagedMqttClient>();
                var options = context.Resolve<MqttSettings>();
                return new ApiMqttPub(logger, client, options);
            })
            .As<IMqttExplorer>()
            .SingleInstance();
        builder.Register(context =>
        {
            var logger = context.Resolve<ILogger<MqttHelp>>();
            var apiMqttPub = context.Resolve<IMqttExplorer>();
            return new MqttHelp(logger,apiMqttPub);
        })
        .As<MqttHelp>()
        .SingleInstance();
        #endregion

        // base.Load(builder);
    }
}