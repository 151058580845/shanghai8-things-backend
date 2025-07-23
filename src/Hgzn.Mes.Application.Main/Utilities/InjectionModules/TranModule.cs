using Autofac;
using Hgzn.Mes.Application.Main.Services.App;
using Hgzn.Mes.Application.Main.Services.App.IService;
using Hgzn.Mes.Infrastructure.Utilities.CurrentUser;

namespace Hgzn.Mes.Application.Main.Utilities.InjectionModules;

public class TranModule:Module
{
    protected override void Load(ContainerBuilder builder)
    {
        
        #region 注册ICurrentUser

        builder.RegisterType<CurrentUser>().As<ICurrentUser>().InstancePerDependency();

        builder.RegisterType<AppService>().As<IAppService>().InstancePerDependency();

        #endregion
        base.Load(builder);
    }
}