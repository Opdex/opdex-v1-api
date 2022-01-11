using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.WebApi.Models.Requests.Markets;

namespace Opdex.Platform.WebApi.Validation.Markets;

public class MarketFilterParametersValidator : AbstractCursorValidator<MarketFilterParameters, MarketsCursor>
{
    public MarketFilterParametersValidator()
    {
        RuleFor(filter => filter.MarketType).MustBeValidEnumValue().WithMessage("Market type must be valid for the enumeration values.");
        RuleFor(filter => filter.OrderBy).MustBeValidEnumValue().WithMessage("Order must be valid for the enumeration values.");
        RuleFor(filter => filter.Direction).MustBeValidEnumValueOrDefault().WithMessage($"Direction must be valid for the enumeration values.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
