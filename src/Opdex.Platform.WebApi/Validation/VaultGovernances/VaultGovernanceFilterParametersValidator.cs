using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultGovernanceFilterParametersValidator : AbstractCursorValidator<VaultGovernanceFilterParameters, VaultGovernancesCursor>
{
    public VaultGovernanceFilterParametersValidator()
    {
        RuleFor(request => request.LockedToken).MustBeNetworkAddressOrEmpty();
    }
}
