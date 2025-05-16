using Autofac;
using Hgzn.Mes.Application.Main.Services;
using Hgzn.Mes.Application.Main.Services.Equip;
using Hgzn.Mes.Application.Main.Services.Equip.IService;
using Hgzn.Mes.Domain.Services;
using Hgzn.Mes.Domain.Services.System;
using Hgzn.Mes.Infrastructure.Utilities.TestDataReceiver.ZXWL_XT_307.ZXWL_SL_1;
using SqlSugar;
using System.Reflection;

namespace Hgzn.Mes.Application.Main.Utilities.InjectionModules
{
    public class AppServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("Hgzn.Mes." + nameof(Application) + ".Main"))
                .Where(type => type.IsAssignableTo(typeof(IBaseService)))
                .AsImplementedInterfaces()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(Assembly.Load("Hgzn.Mes." + nameof(Domain)), Assembly.Load("Hgzn.Mes." + nameof(Infrastructure)))
                .Where(type => type.IsAssignableTo<IDomainService>())
                .AsImplementedInterfaces()
                .PropertiesAutowired();

            builder.RegisterType<EquipLedgerService>().InstancePerLifetimeScope();
            // builder.RegisterType<TestAnalyseJob>().InstancePerLifetimeScope();
        }
    }
}