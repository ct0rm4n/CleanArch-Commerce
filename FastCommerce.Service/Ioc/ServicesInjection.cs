using Microsoft.Extensions.DependencyInjection;
using Service.Factory;

namespace Service.Ioc
{
    public static class ServicesInjection
    {
        public static void InjectConfigureServices(this IServiceCollection builder)
        {
            builder.AddAutoMapper(typeof(AddressProfile));
        }
    }
}
