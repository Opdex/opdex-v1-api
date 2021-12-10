using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class RemoveLiquidityQuoteRequestValidator : AbstractValidator<RemoveLiquidityQuoteRequest>
{
    public RemoveLiquidityQuoteRequestValidator()
    {
        RuleFor(request => request.AmountCrsMin).MustBeValidCrsValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.AmountSrcMin).MustBeValidSrcValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.Liquidity).MustBeValidSrcValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.Recipient).MustBeNetworkAddress();
    }
}
