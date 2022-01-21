using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Markets;

namespace Opdex.Platform.WebApi.Validation.Markets;

public class SetMarketOwnerQuoteRequestValidator : AbstractValidator<SetMarketOwnerQuoteRequest>
{
    public SetMarketOwnerQuoteRequestValidator()
    {
        RuleFor(r => r.Owner)
            .MustBeNetworkAddress()
            .WithMessage("Owner must be valid address");
    }
}
