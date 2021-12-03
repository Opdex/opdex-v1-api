using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using Opdex.Platform.WebApi.Models.Requests.MiningGovernances;

namespace Opdex.Platform.WebApi.Validation.MiningGovernances;

public class MiningGovernanceFilterParametersValidator : AbstractCursorValidator<MiningGovernanceFilterParameters, MiningGovernancesCursor>
{
    public MiningGovernanceFilterParametersValidator()
    {
        RuleFor(filter => filter.MinedToken).MustBeNetworkAddressOrEmpty();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}