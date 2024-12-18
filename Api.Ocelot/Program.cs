using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using MMLib.SwaggerForOcelot.DependencyInjection;
using SwaggerOcelot.Gateway;

namespace Api.Ocelot
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration(config =>
            {
                config.AddOcelotWithSwaggerSupport(options =>
                {
                    options.Folder = "OcelotConfiguration";
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>().UseUrls(new string[] { "http://localhost:5000" });
            });
    }
}
