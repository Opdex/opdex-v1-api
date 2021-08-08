using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Opdex.Platform.WebApi.Models.Responses
{
    public class ValidationErrorProblemDetailsResult : UnprocessableEntityObjectResult
    {
        public ValidationErrorProblemDetailsResult(params string[] errors) : base(CreateProblemDetails(errors))
        {
        }

        private static ProblemDetails CreateProblemDetails(string[] errors)
        {
            if (errors is null || errors.Length == 0) throw new ArgumentNullException(nameof(errors), "Must have one or more errors.");

            var problemDetails = new ProblemDetails
            {
                Title = "Unprocessable Entity",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "A validation error occurred.",
                Type = "https://httpstatuses.com/422"
            };
            problemDetails.Extensions.Add("Errors", errors);
            return problemDetails;
        }
    }
}
