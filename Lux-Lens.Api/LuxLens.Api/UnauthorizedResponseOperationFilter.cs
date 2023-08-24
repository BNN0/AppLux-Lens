using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LuxLens.Api
{
    public class UnauthorizedResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Responses.ContainsKey("401"))
            {
                operation.Responses["401"] = new OpenApiResponse
                {
                    Description = "No autorizado - La solicitud no tiene permisos suficientes."
                };
            }
        }
    }
}
