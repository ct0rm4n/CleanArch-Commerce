using Autofac;
using Data.Commands.Data;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Module = Autofac.Module;

namespace Data.Ioc
{
    public class AutofacDataModule : Module
    {
        
        protected override void Load(ContainerBuilder builder)
        {
            
            builder.RegisterGeneric(typeof(GenericRepository<,>))
                .As(typeof(IGenericRepository<,>)).InstancePerLifetimeScope();

            var dataAccess = Assembly.GetExecutingAssembly();
            Assembly repoServiceAssembly = Assembly.GetAssembly(typeof(IConfiguration));

            builder.RegisterAssemblyTypes(dataAccess)
               .Where(t => (t.Name.EndsWith("Repository") ))
               .InstancePerLifetimeScope().WithParameter((pi, ctx) => pi.ParameterType == typeof(IConfiguration) && pi.Name == "configuration",
                          (pi, ctx) => ctx.Resolve<IConfiguration>());

            builder.RegisterAssemblyTypes(dataAccess)
               .Where(t => (t.Name.EndsWith("Helper") || (t.Name.EndsWith("Service") && !t.Name.Equals("IGenericRepository"))))
               .AsImplementedInterfaces().InstancePerLifetimeScope();

        }
    }
}