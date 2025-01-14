using Autofac;
using Hgzn.Mes.Infrastructure.DbContexts.Ef;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlSugar;
using StackExchange.Redis;

namespace Hgzn.Mes.Application.Main.Utilities.InjectionModules
{
    public class SingletonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new LoggerFactory().CreateLogger("Adapter"))
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<InitialDatabase>()
                .SingleInstance();
            builder.Register(context => ConnectionMultiplexer.Connect(context.Resolve<IConfiguration>().GetConnectionString("Redis")!))
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterGeneric(typeof(PaginatedListConverter<,>));
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
        }
    }
}