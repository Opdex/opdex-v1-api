using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Tokens;

namespace Opdex.Platform.WebApi.Validation.Tokens;

public class ApproveAllowanceQuoteRequestValidator : AbstractValidator<ApproveAllowanceQuoteRequest>
{
    public ApproveAllowanceQuoteRequestValidator()
    {
        RuleFor(request => request.Amount).MustBeValidSrcValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.Spender).MustBeNetworkAddress();
    }
}
