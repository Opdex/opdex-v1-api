using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class MinimumVoteVaultProposalQuoteRequestValidator : AbstractValidator<MinimumVoteVaultProposalQuoteRequest>
{
    public MinimumVoteVaultProposalQuoteRequestValidator()
    {
        RuleFor(request => request.Amount).MustBeValidCrsValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(200);
    }
}
