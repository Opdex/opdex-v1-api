using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;

namespace Opdex.Platform.WebApi.Validation.MiningPools;

public class MiningQuoteValidator : AbstractValidator<MiningQuote>
{
    public MiningQuoteValidator()
    {
        RuleFor(request => request.Amount)
            .MustBeValidLptValue().WithMessage("Amount must contain 8 decimal places.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Amount must be greater than 0.");
    }
}
