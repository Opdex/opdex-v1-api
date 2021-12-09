using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class StopStakingQuoteRequestValidator : AbstractValidator<StopStakingQuoteRequest>
{
    public StopStakingQuoteRequestValidator()
    {
        RuleFor(request => request.Amount).MustBeValidSrcValue().GreaterThan(FixedDecimal.Zero);
    }
}
