using Autofac;
using Hgzn.Mes.Application.Services.Base;
using Hgzn.Mes.Domain.Services;
using System.Reflection;
using Hgzn.Mes.Application.Services;

namespace Hgzn.Mes.WebApi.Utilities.InjectionModules
{
    public class AppServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("Hgzn.Mes." + nameof(Application)))
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