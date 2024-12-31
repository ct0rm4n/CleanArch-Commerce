using System.Net;

namespace Service.Filter
{
    public class AuthorizationActionFilter : IEndpointFilter
    {

        private readonly string _apiKey;
        private readonly string _apiKeySecondary;
        private readonly bool _canUseSecondaryApiKey;

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            string _apiKey = configuration["SecretKeys:ApiKey"];
            string _apiKeySecondary = configuration["SecretKeys:ApiKeySecondary"];
            string _canUseSecondaryApiKey = configuration["SecretKeys:UseSecondaryKey"];
            var apiKeyHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (apiKeyHeader.Any())
            {
                var keys = new List<string>
                {
                    _apiKey
                };

                if (_canUseSecondaryApiKey is "True")
                {
                    keys.AddRange(_apiKeySecondary.Split(','));
                }

                if (keys.FindIndex(x => x.Equals(apiKeyHeader, StringComparison.OrdinalIgnoreCase)) == -1)
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                }
            }
            else
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }

            return await next(context);
        }

    }
    
}
