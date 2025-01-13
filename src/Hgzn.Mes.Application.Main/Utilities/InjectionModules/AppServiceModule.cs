using System.Reflection;
using Autofac;
using Hgzn.Mes.Application.Main.Services;
using Hgzn.Mes.Domain.Services;
using SqlSugar;

namespace Hgzn.Mes.Application.Main.Utilities.InjectionModules
{
    public class AppServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("Hgzn.Mes." + nameof(Application)+".Main"))
                .Where(type => type.IsAssignableTo(typeof(IBaseService)))
                .AsImplementedInterfaces()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(Assembly.Load("Hgzn.Mes." + nameof(Domain)), Assembly.Load("Hgzn.Mes." + nameof(Infrastructure)))
                .Where(type => type.IsAssignableTo<IDomainService>())
                .AsImplementedInterfaces()
                .PropertiesAutowired();
        }
    }
}