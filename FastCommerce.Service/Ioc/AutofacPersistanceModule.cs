using Autofac;
using Autofac.Core;
using FastCommerce.Core.Data;
using FastCommerce.Data.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Module = Autofac.Module;

namespace FastCommerce.Data.Ioc
{
    public class AutofacPersistanceModule : Module
    {
        
        protected override void Load(ContainerBuilder builder)
        {
            
            var dataAccess = Assembly.GetExecutingAssembly();

            Assembly repoServiceAssembly = Assembly.GetAssembly(typeof(DbContext));

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces();
        }
    }
}