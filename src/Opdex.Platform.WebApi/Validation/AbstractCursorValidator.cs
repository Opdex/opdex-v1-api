using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests;

namespace Opdex.Platform.WebApi.Validation;

public abstract class AbstractCursorValidator<T, TCursor> : AbstractValidator<T>, IValidatorInterceptor where T : FilterParameters<TCursor>
    where TCursor : Cursor
{
    protected AbstractCursorValidator()
    {
        When(filter => filter.EncodedCursor.HasValue(),
             () => RuleFor(filter => filter.EncodedCursor).Must((filter, cursor) => filter.ValidateWellFormed()).WithMessage("Cursor not formed correctly."));
    }

    public ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
    {
        return result;
    }

    public IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
    {
        var queryParams = actionContext.HttpContext.Request.Query;
        if (!queryParams.ContainsKey("cursor") || queryParams.Count == 1)
        {
            return commonContext;
        }

        foreach (var queryParam in queryParams.Keys)
        {
            if (queryParam.EqualsIgnoreCase("cursor")) continue;
            actionContext.ModelState.AddModelError(queryParam, "Filters cannot be provided alongside cursor.");
        }

        return commonContext;
    }
}
