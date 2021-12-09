using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class AddLiquidityQuoteRequestValidator : AbstractValidator<AddLiquidityQuoteRequest>
{
    public AddLiquidityQuoteRequestValidator()
    {
        RuleFor(request => request.AmountCrs).MustBeValidCrsValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.AmountCrsMin).MustBeValidCrsValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.AmountSrc).MustBeValidSrcValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.AmountSrcMin).MustBeValidSrcValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.Recipient).MustBeNetworkAddress();
    }
}
