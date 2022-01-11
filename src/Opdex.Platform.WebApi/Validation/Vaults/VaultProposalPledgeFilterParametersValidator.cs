using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class VaultProposalPledgeFilterParametersValidator : AbstractCursorValidator<VaultProposalPledgeFilterParameters, VaultProposalPledgesCursor>
{
    public VaultProposalPledgeFilterParametersValidator()
    {
        RuleFor(filter => filter.Pledger).MustBeNetworkAddressOrEmpty().WithMessage("Pledger must be valid address.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
