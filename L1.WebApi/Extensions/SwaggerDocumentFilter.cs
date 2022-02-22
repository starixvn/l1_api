using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace L1.WebApi.Extensions
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var apiDescription in context.ApiDescriptions)
            {
                var actionDescriptor = apiDescription.ActionDescriptor as ControllerActionDescriptor;

                if (actionDescriptor.ControllerTypeInfo.GetCustomAttributes(typeof(SwaggerIgnoreAttribute), true).Any() ||
                   actionDescriptor.MethodInfo.GetCustomAttributes(typeof(SwaggerIgnoreAttribute), true).Any())
                {
                    var key = $"/{apiDescription.RelativePath.TrimEnd('/')}";
                    swaggerDoc.Paths.Remove(key);
                }
            }
        }
    }
}