using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.MarketTokens;

namespace Opdex.Platform.WebApi.Validation.MarketTokens;

public class SwapRequestValidator : AbstractValidator<SwapRequest>
{
    public SwapRequestValidator()
    {
        RuleFor(r => r.TokenOut)
            .MustBeNetworkAddressOrCrs().WithMessage("Token out must be valid address or CRS.");
        RuleFor(r => r.TokenInAmount)
            .MustBeValidSrcValue().WithMessage("Token in amount must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Token in amount must be greater than zero.");
        RuleFor(r => r.TokenOutAmount)
            .MustBeValidSrcValue().WithMessage("Token out amount must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Token out amount must be greater than zero.");
        RuleFor(r => r.TokenInMaximumAmount)
            .MustBeValidSrcValue().WithMessage("Token in maximum amount must contain 18 decimal places or less.")
            .GreaterThanOrEqualTo(req => req.TokenInAmount).WithMessage("Token in maximum amount cannot be less than token in amount.");
        When(r => r.TokenInExactAmount, () =>
        {
            RuleFor(r => r.TokenInMaximumAmount).Must((req, tokenInMaximumAmount) => tokenInMaximumAmount == req.TokenInAmount)
                                                .WithMessage("Token in maximum amount must equal token in amount, since exact amount is specified.");
        });
        RuleFor(r => r.TokenOutMinimumAmount)
            .MustBeValidSrcValue().WithMessage("Token out minimum amount must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Token out minimum amount must be greater than zero.")
            .LessThanOrEqualTo(req => req.TokenOutAmount).WithMessage("Token out minimum amount cannot be greater than token out amount.");
        RuleFor(r => r.Recipient)
            .MustBeNetworkAddress().WithMessage("Recipient must be valid address.");
    }
}
