using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses
{
    [Obsolete("Validation can be done outside of cursor, using fluent validation.")]
    public class ValidationErrorProblemDetailsResult : UnprocessableEntityObjectResult
    {
        public ValidationErrorProblemDetailsResult(string key, params string[] value) : base(CreateProblemDetails(key, value))
        {
        }

        public static ProblemDetails CreateProblemDetails(string key, params string[] value) => CreateProblemDetails(new Dictionary<string, string[]> { { key, value } });

        public static ProblemDetails CreateProblemDetails(IDictionary<string, string[]> errors)
        {
            if (errors is null || errors.Count == 0) throw new ArgumentNullException(nameof(errors), "Must have one or more errors.");

            var problemDetails = new ValidationProblemDetails
            {
                Title = "Unprocessable Entity",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "A validation error occurred.",
                Type = "https://httpstatuses.com/422"
            };

            foreach (var pair in errors) problemDetails.Errors.Add(pair);

            return problemDetails;
        }
    }
}
