using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Opdex.Platform.WebApi.OpenApi;

/// <summary>
/// Applies a 500 Internal Server Error response to an OpenAPI operation.
/// </summary>
public class InternalServerErrorOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var problemDetailsSchema = context.SchemaResolver.GetSchema(typeof(ProblemDetails), false);

        var response = new OpenApiResponse
        {
            Description = "Unexpected error occurred."
        };
        response.Content.Add("application/json", new OpenApiMediaType
        {
            Schema = new JsonSchema
            {
                Reference = problemDetailsSchema,
            }
        });

        context.OperationDescription.Operation.Responses.Add(StatusCodes.Status500InternalServerError.ToString(), response);
        return true;
    }
}
