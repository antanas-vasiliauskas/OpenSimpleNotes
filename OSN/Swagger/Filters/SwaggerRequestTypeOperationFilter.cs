using Microsoft.OpenApi.Models;
using OSN.Swagger.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace OSN.Swagger.Filters;

public class SwaggerRequestTypeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attribute = context.MethodInfo.GetCustomAttribute<SwaggerRequestTypeAttribute>();

        if (attribute is null)
        {
            return;
        }

        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["application/json"] = new OpenApiMediaType
                {
                    Schema = context.SchemaGenerator.GenerateSchema(attribute.Type, context.SchemaRepository)
                }
            }
        };
    }
}