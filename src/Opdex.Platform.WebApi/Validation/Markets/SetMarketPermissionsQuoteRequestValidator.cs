using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Markets;

namespace Opdex.Platform.WebApi.Validation.Markets;

public class SetMarketPermissionsQuoteRequestValidator : AbstractValidator<SetMarketPermissionsQuoteRequest>
{
    public SetMarketPermissionsQuoteRequestValidator()
    {
        RuleFor(r => r.Permission)
            .MustBeValidEnumValue()
            .WithMessage("Permission must be valid for the enumeration values.");
    }
}
