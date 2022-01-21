using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Markets;

namespace Opdex.Platform.WebApi.Validation.Markets;

public class CollectMarketFeesQuoteRequestValidator : AbstractValidator<CollectMarketFeesQuoteRequest>
{
    public CollectMarketFeesQuoteRequestValidator()
    {
        RuleFor(r => r.Token)
            .MustBeNetworkAddress()
            .WithMessage("Token must be valid address");

        RuleFor(r => r.Amount)
            .MustBeValidSrcValue().WithMessage("Amount must contain 18 decimal places or less")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Amount must be greater than zero");
    }
}
