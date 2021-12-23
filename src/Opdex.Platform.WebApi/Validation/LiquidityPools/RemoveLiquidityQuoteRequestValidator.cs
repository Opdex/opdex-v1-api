using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class RemoveLiquidityQuoteRequestValidator : AbstractValidator<RemoveLiquidityQuoteRequest>
{
    public RemoveLiquidityQuoteRequestValidator()
    {
        RuleFor(request => request.AmountCrsMin)
            .MustBeValidCrsValue().WithMessage("Minimum CRS amount must contain 8 decimal places.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Minimum CRS amount must be greater than 0.");
        RuleFor(request => request.AmountSrcMin)
            .MustBeValidSrcValue().WithMessage("Minimum SRC amount must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Minimum SRC amount must be greater than 0.");
        RuleFor(request => request.Liquidity)
            .MustBeValidLptValue().WithMessage("Liquidity amount must contain 8 decimal places.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Liquidity amount must be greater than 0.");
        RuleFor(request => request.Recipient)
            .MustBeNetworkAddress().WithMessage("Recipient must be valid address.");
    }
}
