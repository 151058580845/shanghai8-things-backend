using Autofac;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
<<<<<<< HEAD
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
=======
>>>>>>> xa
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Utilities.InjectionModules;

public class ScopeModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        #region 注册sqlSugar
<<<<<<< HEAD
        
=======

>>>>>>> xa
        builder.Register(context =>
            {
                var setting = context.Resolve<IConfiguration>().GetSection(nameof(DbConnOptions))
                    .Get<DbConnOptions>() ?? throw new Exception("sqlsugar config not found!");
                return new SqlSugarClient(SqlSugarContext.Build(setting));
            })
            .As<ISqlSugarClient>()
            .InstancePerLifetimeScope();
<<<<<<< HEAD
        builder.Register(context =>
            {
                var setting = context.Resolve<IConfiguration>().GetSection(nameof(DbConnOptions))
                    .Get<DbConnOptions>() ?? throw new Exception("sqlsugar config not found!");
                var logger = context.Resolve<ILogger<SqlSugarContext>>();
                var client = context.Resolve<ISqlSugarClient>();
                var user = context.Resolve<ICurrentUser>();
                return new SqlSugarContext(logger, setting,client, user);
            })
            .PropertiesAutowired()
            .InstancePerLifetimeScope();
        #endregion

=======
        #endregion
>>>>>>> xa
        base.Load(builder);
    }
}