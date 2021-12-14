using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultProposalVoteFilterParametersValidator : AbstractCursorValidator<VaultProposalVoteFilterParameters, VaultProposalVotesCursor>
{
    public VaultProposalVoteFilterParametersValidator()
    {
        RuleFor(filter => filter.Voter).MustBeNetworkAddressOrEmpty();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}
