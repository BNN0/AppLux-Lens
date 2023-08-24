using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LuxLens.Api
{
    public class CustomAuthorizeFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new JsonResult("No estás autenticado.")
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            // Verifica si el usuario tiene los roles necesarios, si es necesario
            // ...

            await Task.CompletedTask;
        }
    }
}