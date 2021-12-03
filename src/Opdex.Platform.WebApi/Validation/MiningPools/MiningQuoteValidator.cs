using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;

namespace Opdex.Platform.WebApi.Validation.MiningPools;

public class MiningQuoteValidator : AbstractValidator<MiningQuote>
{
    public MiningQuoteValidator()
    {
        RuleFor(request => request.Amount).MustBeValidTokenValue().GreaterThan(FixedDecimal.Zero);
    }
}