using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Opdex.Platform.WebApi.OpenApi;

/// <summary>
/// Applies a 429 Too Many Requests response to an OpenAPI operation.
/// </summary>
public class TooManyRequestErrorOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var problemDetailsSchema = context.SchemaResolver.GetSchema(typeof(ProblemDetails), false);

        var response = new OpenApiResponse
        {
            Description = "Too many requests."
        };
        response.Content.Add("application/json", new OpenApiMediaType
        {
            Schema = new JsonSchema
            {
                Reference = problemDetailsSchema,
            }
        });

        context.OperationDescription.Operation.Responses.Add(StatusCodes.Status429TooManyRequests.ToString(), response);
        return true;
    }
}
