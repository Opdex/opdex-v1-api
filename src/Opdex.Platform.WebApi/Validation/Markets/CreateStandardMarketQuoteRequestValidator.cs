using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.Markets;

namespace Opdex.Platform.WebApi.Validation.Markets;

public class CreateStandardMarketQuoteRequestValidator : AbstractValidator<CreateStandardMarketQuoteRequest>
{
    public CreateStandardMarketQuoteRequestValidator()
    {
        RuleFor(r => r.Owner)
            .MustBeNetworkAddress().WithMessage("Owner must be valid address");
        RuleFor(r => r.TransactionFeePercent)
            .GreaterThanOrEqualTo(0.0m).WithMessage("Transaction fee cannot be negative")
            .LessThanOrEqualTo(1.0m).WithMessage("Transaction fee cannot exceed 1%")
            .Must(fee => decimal.Round(fee, 1) == fee).WithMessage("Transaction fee can only increment in steps of 0.1%");
        When(r => r.TransactionFeePercent == 0m, () =>
        {
            RuleFor(r => r.EnableMarketFee)
                .Must(enabled => !enabled)
                .WithMessage("Market fee must be disabled if transaction fee is 0%");
        });
    }
}
