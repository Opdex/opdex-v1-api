using FluentValidation;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class CreateLiquidityPoolQuoteRequestValidator : AbstractValidator<CreateLiquidityPoolQuoteRequest>
{
    public CreateLiquidityPoolQuoteRequestValidator()
    {
        RuleFor(request => request.Market).MustBeNetworkAddress();
        RuleFor(request => request.Token).MustBeNetworkAddress();
    }
}
