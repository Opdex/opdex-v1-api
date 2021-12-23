using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class MinimumVoteVaultProposalQuoteRequestValidator : AbstractValidator<MinimumVoteVaultProposalQuoteRequest>
{
    public MinimumVoteVaultProposalQuoteRequestValidator()
    {
        RuleFor(request => request.Amount)
            .MustBeValidCrsValue().WithMessage("Amount must contain 8 decimal places.")
            .GreaterThan(FixedDecimal.Zero).WithMessage("Amount must be greater than 0.");
        RuleFor(request => request.Description)
            .NotEmpty().WithMessage("Description must not be empty.")
            .MaximumLength(200).WithMessage("Description can contain a maximum of 200 characters.");
    }
}
