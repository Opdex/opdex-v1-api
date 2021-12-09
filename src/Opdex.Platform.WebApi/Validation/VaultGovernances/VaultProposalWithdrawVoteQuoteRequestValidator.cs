using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultProposalWithdrawVoteQuoteRequestValidator : AbstractValidator<VaultProposalWithdrawVoteQuoteRequest>
{
    public VaultProposalWithdrawVoteQuoteRequestValidator()
    {
        RuleFor(request => request.Amount).MustBeValidCrsValue().GreaterThan(FixedDecimal.Zero);
    }
}
