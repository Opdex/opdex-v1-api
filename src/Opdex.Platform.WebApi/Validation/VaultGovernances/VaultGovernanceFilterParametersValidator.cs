using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;

namespace Opdex.Platform.WebApi.Validation.VaultGovernances;

public class VaultGovernanceFilterParametersValidator : AbstractCursorValidator<VaultGovernanceFilterParameters, VaultGovernancesCursor>
{
    public VaultGovernanceFilterParametersValidator()
    {
        RuleFor(request => request.LockedToken).MustBeNetworkAddressOrEmpty().WithMessage("Locked token must be valid address.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
