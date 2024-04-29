using FastCommerce.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FastCommerce.Service.Ioc
{
    public static class ServicesInjection
    {
        public static void InjectConfigureServices(this IServiceCollection builder)
        {
            builder.AddScoped<IUserService, UserService>();
        }
    }
}
