using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class VaultProposalVoteQuoteRequestValidator : AbstractValidator<VaultProposalVoteQuoteRequest>
{
    public VaultProposalVoteQuoteRequestValidator()
    {
        RuleFor(request => request.Amount)
            .MustBeValidCrsValue().WithMessage("Amount must contain 8 decimal places.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Amount must be greater than 0.");
    }
}
