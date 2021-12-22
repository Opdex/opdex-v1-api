using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class AddLiquidityQuoteRequestValidator : AbstractValidator<AddLiquidityQuoteRequest>
{
    public AddLiquidityQuoteRequestValidator()
    {
        RuleFor(request => request.AmountCrs)
            .MustBeValidCrsValue().WithMessage("CRS amount must contain 8 decimal places.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("CRS amount must be greater than 0.");
        RuleFor(request => request.AmountCrsMin)
            .MustBeValidCrsValue().WithMessage("Minimum CRS amount must contain 8 decimal places.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Minimum CRS amount must be greater than 0.");
        RuleFor(request => request.AmountSrc)
            .MustBeValidSrcValue().WithMessage("SRC amount must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("SRC amount must be greater than 0.");
        RuleFor(request => request.AmountSrcMin)
            .MustBeValidSrcValue().WithMessage("Minimum SRC amount must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Minimum SRC amount must be greater than 0.");
        RuleFor(request => request.Recipient)
            .MustBeNetworkAddress().WithMessage("Recipient must be valid address.");
    }
}
