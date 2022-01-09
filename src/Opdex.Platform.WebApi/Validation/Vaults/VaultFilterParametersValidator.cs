using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class VaultFilterParametersValidator : AbstractCursorValidator<VaultFilterParameters, VaultsCursor>
{
    public VaultFilterParametersValidator()
    {
        RuleFor(request => request.LockedToken).MustBeNetworkAddressOrEmpty().WithMessage("Locked token must be valid address.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
