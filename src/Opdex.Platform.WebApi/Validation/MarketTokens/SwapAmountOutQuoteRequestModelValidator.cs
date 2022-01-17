using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.MarketTokens;

namespace Opdex.Platform.WebApi.Validation.MarketTokens;

public class SwapAmountOutQuoteRequestModelValidator : AbstractValidator<SwapAmountOutQuoteRequestModel>
{
    public SwapAmountOutQuoteRequestModelValidator()
    {
        RuleFor(r => r.TokenIn)
            .MustBeNetworkAddressOrCrs().WithMessage("Token in must be valid address or CRS.");
        RuleFor(r => r.TokenInAmount)
            .MustBeValidSrcValue().WithMessage("Token in amount must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Token in amount must be greater than zero.");
    }
}
