using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;

namespace Opdex.Platform.WebApi.Validation.MiningPools;

public class MiningQuoteValidator : AbstractValidator<MiningQuote>
{
    public MiningQuoteValidator()
    {
        RuleFor(request => request.Amount)
            .MustBeValidSrcValue().WithMessage("Amount must be SRC value with 18 decimal places or less.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Amount must be greater than 0.");
    }
}
