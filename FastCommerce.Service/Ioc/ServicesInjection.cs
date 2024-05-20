using Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Ioc
{
    public static class ServicesInjection
    {
        public static void InjectConfigureServices(this IServiceCollection builder)
        {
            builder.AddScoped<IUserService, UserService>();
        }
    }
}
