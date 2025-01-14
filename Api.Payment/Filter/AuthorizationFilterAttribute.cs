using Service.Interfaces;
using System.Net;

namespace Service.Filter
{
    public class AuthorizationLoggedActionFilter : IEndpointFilter
    {       

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
            var auth_validate = authService.LoginValidate(context.HttpContext.Request.Headers["Authorization"].ToString());
            if (auth_validate.Token is null && !auth_validate.Logged)
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            return await next(context);
        }

    }
}
