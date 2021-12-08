using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class MinimumPledgeVaultProposalQuoteRequestValidator : AbstractValidator<MinimumPledgeVaultProposalQuoteRequest>
{
    public MinimumPledgeVaultProposalQuoteRequestValidator()
    {
        RuleFor(request => request.Amount).MustBeValidCrsValue().GreaterThan(FixedDecimal.Zero);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(200);
    }
}
