using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using Opdex.Platform.WebApi.Models.Requests.Vaults;

namespace Opdex.Platform.WebApi.Validation.Vaults;

public class VaultFilterParametersValidator : AbstractCursorValidator<VaultFilterParameters, VaultsCursor>
{
    public VaultFilterParametersValidator()
    {
        RuleFor(filter => filter.LockedToken).MustBeNetworkAddressOrEmpty();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}