using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultProposalPledgeFilterParametersValidator : AbstractCursorValidator<VaultProposalPledgeFilterParameters, VaultProposalPledgesCursor>
{
    public VaultProposalPledgeFilterParametersValidator()
    {
        RuleFor(filter => filter.Pledger).MustBeNetworkAddressOrEmpty();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}
