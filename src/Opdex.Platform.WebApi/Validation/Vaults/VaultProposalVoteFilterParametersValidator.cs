using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Votes;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class VaultProposalVoteFilterParametersValidator : AbstractCursorValidator<VaultProposalVoteFilterParameters, VaultProposalVotesCursor>
{
    public VaultProposalVoteFilterParametersValidator()
    {
        RuleFor(filter => filter.Voter).MustBeNetworkAddressOrEmpty().WithMessage("Voter must be valid address.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
