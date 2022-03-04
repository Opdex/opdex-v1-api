using FluentValidation;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests;

namespace Opdex.Platform.WebApi.Validation;

public abstract class AbstractCursorValidator<T, TCursor> : AbstractValidator<T>
    where T : FilterParameters<TCursor>
    where TCursor : Cursor
{
    protected AbstractCursorValidator()
    {
        When(filter => filter.EncodedCursor.HasValue(),
             () => RuleFor(filter => filter.EncodedCursor).Must((filter, cursor) => filter.ValidateWellFormed()).WithMessage("Cursor not formed correctly."));
    }
}
