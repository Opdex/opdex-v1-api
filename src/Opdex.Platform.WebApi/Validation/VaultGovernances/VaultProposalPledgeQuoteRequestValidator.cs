using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultProposalPledgeQuoteRequestValidator : AbstractValidator<VaultProposalPledgeQuoteRequest>
{
    public VaultProposalPledgeQuoteRequestValidator()
    {
        RuleFor(request => request.Amount)
            .MustBeValidCrsValue().WithMessage("Amount must contain 8 decimal places.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Amount must be greater than 0.");
    }
}
