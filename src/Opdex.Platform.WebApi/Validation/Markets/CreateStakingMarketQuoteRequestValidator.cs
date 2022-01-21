using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Markets;

namespace Opdex.Platform.WebApi.Validation.Markets;

public class CreateStakingMarketQuoteRequestValidator : AbstractValidator<CreateStakingMarketQuoteRequest>
{
    public CreateStakingMarketQuoteRequestValidator()
    {
        RuleFor(r => r.StakingToken)
            .MustBeNetworkAddress()
            .WithMessage("Staking token must be valid address");
    }
}
