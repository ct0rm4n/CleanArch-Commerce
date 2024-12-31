using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Service.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string apiKey = _configuration["SecretKeys:ApiKey"];
            string apiKeySecondary = _configuration["SecretKeys:ApiKeySecondary"];
            bool canUseSecondaryApiKey = bool.Parse(_configuration["SecretKeys:UseSecondaryKey"]);
            var apiKeyHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(apiKeyHeader))
            {
                var keys = new List<string> { apiKey };

                if (canUseSecondaryApiKey)
                {
                    keys.AddRange(apiKeySecondary.Split(','));
                }

                if (!keys.Any(key => key.Equals(apiKeyHeader, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            await _next(context);
        }
    }
}