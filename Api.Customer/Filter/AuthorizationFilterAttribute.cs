using System.Net;

namespace Service.Filter
{
    public class AuthorizationActionFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiValidatorMiddleware = new Service.Middleware.AuthorizationMiddleware(configuration);
            var validate = await apiValidatorMiddleware.ValidApiKey(context.HttpContext);

            return await next(context);
        }

    }
    
}
