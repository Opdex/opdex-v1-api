using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Tokens;

namespace Opdex.Platform.WebApi.Validation.Tokens;

public class ApproveAllowanceQuoteRequestValidator : AbstractValidator<ApproveAllowanceQuoteRequest>
{
    public ApproveAllowanceQuoteRequestValidator()
    {
        RuleFor(request => request.Amount)
            .MustBeValidSrcValue().WithMessage("Amount must contain 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Amount must be greater than 0.");
        RuleFor(request => request.Spender).MustBeNetworkAddress().WithMessage("Spender must be valid address.");
    }
}
