using Autofac;
using HgznMes.Application.Services.Base;
using HgznMes.Domain.Services;
using System.Reflection;
using HgznMes.Application.Services;

namespace HgznMes.WebApi.Utilities.InjectionModules
{
    public class AppServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("HgznMes." + nameof(Application)))
                .Where(type => type.IsAssignableTo(typeof(IBaseService)))
                .AsImplementedInterfaces()
                .PropertiesAutowired();

            builder.RegisterAssemblyTypes(Assembly.Load("HgznMes." + nameof(Domain)), Assembly.Load("HgznMes." + nameof(Infrastructure)))
                .Where(type => type.IsAssignableTo<IDomainService>())
                .AsImplementedInterfaces()
                .PropertiesAutowired();
        }
    }
}