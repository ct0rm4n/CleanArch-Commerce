using Autofac;
using Autofac.Core;
using Core.Data;
using Data.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace Data.Ioc
{
    public class AutofacPersistanceModule : Module
    {
        
        protected override void Load(ContainerBuilder builder)
        {
            
            var dataAccess = Assembly.GetExecutingAssembly();

            Assembly repoServiceAssembly = Assembly.GetAssembly(typeof(IConfiguration));

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();
        }
    }
}