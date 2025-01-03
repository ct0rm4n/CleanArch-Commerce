using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Service.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly IConfiguration _configuration;

        public AuthorizationMiddleware(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(bool isValid, HttpContext context)> ValidApiKey(HttpContext context)
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
                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("Unauthorized");
                    }
                    return (false, context);
                }
            }
            else
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                }
                return (false, context);
            }

            return (true, context);
        }
    }
}