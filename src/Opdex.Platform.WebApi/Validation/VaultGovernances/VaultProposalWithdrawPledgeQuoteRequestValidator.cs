using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultProposalWithdrawPledgeQuoteRequestValidator : AbstractValidator<VaultProposalWithdrawPledgeQuoteRequest>
{
    public VaultProposalWithdrawPledgeQuoteRequestValidator()
    {
        RuleFor(request => request.Amount).MustBeValidCrsValue().GreaterThan(FixedDecimal.Zero);
    }
}
