using Autofac;
using Hgzn.Mes.Infrastructure.DbContexts.SqlSugar;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Utilities.InjectionModules;

public class ScopeModule:Module
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
            .InstancePerLifetimeScope();
        #endregion

        base.Load(builder);
    }
}