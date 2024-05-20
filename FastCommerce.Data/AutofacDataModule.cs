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
    public class AutofacDataModule : Module
    {
        
        protected override void Load(ContainerBuilder builder)
        {
            
            builder.RegisterGeneric(typeof(GenericRepository<>))
                .As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();
            var dataAccess = Assembly.GetExecutingAssembly();

            Assembly repoServiceAssembly = Assembly.GetAssembly(typeof(DbContext));

            builder.RegisterAssemblyTypes(dataAccess)
               .Where(t => (t.Name.Equals("GenericRepository") ))
               .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(dataAccess)
               .Where(t => (t.Name.EndsWith("Helper") || (t.Name.EndsWith("Repository") && !t.Name.Equals("GenericRepository") && !t.Name.Equals("IGenericRepository"))))
               .AsImplementedInterfaces().InstancePerLifetimeScope();
            
            builder.Register(c =>
            {
                IConfiguration config = c.Resolve<IConfiguration>();
                return new DbContext(config.GetSection("ConnectionStrings:SqlConnection").Value);
            }).AsSelf().InstancePerLifetimeScope();
        }
    }
}