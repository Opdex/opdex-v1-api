using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using Opdex.Platform.WebApi.Models.Requests.Governances;

namespace Opdex.Platform.WebApi.Validation.Governances
{
    public class GovernanceFilterParametersValidator : AbstractCursorValidator<GovernanceFilterParameters, MiningGovernancesCursor>
    {
        public GovernanceFilterParametersValidator()
        {
            RuleFor(filter => filter.MinedToken).MustBeNetworkAddressOrEmpty();
            RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
        }
    }
}
