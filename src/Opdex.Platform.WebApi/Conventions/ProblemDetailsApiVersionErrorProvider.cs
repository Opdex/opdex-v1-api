using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using System;
using System.Diagnostics;

namespace Opdex.Platform.WebApi.Conventions;

public class ProblemDetailsApiVersionErrorProvider : IErrorResponseProvider
{
    public IActionResult CreateResponse(ErrorResponseContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://httpstatuses.com/400",
            Title = "Bad Request",
            Detail = "Unsupported API version"
        };
        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? context.Request.HttpContext.TraceIdentifier;
        return new BadRequestObjectResult(problemDetails);
    }
}
