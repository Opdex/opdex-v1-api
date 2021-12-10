using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class SkimQuoteRequestValidator : AbstractValidator<SkimQuoteRequest>
{
    public SkimQuoteRequestValidator()
    {
        RuleFor(request => request.Recipient).MustBeNetworkAddress();
    }
}
