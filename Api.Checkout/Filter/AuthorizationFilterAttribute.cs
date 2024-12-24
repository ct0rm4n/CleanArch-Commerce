using Service.Interfaces;
using System.Net;

namespace Service.Filter
{
    public class AuthorizationLoggedActionFilter : IEndpointFilter
    {
        private readonly IAuthService authService;
        public AuthorizationLoggedActionFilter(IAuthService authService)
        {
            this.authService = authService;
        }


        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {            
            var ajwtToken = context.HttpContext.Request.Headers["Authorization"].ToString();
            var auth_validate = authService.LoginValidate(ajwtToken);
            if (auth_validate.Token is null && !auth_validate.Logged)
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;                
            
             return await next(context);
        }

    }
    
}
