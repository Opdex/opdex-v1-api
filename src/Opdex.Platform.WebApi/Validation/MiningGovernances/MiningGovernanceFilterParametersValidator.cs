using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using Opdex.Platform.WebApi.Models.Requests.MiningGovernances;

namespace Opdex.Platform.WebApi.Validation.MiningGovernances;

public class MiningGovernanceFilterParametersValidator : AbstractCursorValidator<MiningGovernanceFilterParameters, MiningGovernancesCursor>
{
    public MiningGovernanceFilterParametersValidator()
    {
        RuleFor(filter => filter.MinedToken).MustBeNetworkAddressOrEmpty().WithMessage("Mined token must be valid address.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
