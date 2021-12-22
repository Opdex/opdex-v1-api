using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using Opdex.Platform.WebApi.Models.Requests.Wallets;

namespace Opdex.Platform.WebApi.Validation.Wallets;

public class StakingPositionFilterParametersValidator : AbstractCursorValidator<StakingPositionFilterParameters, StakingPositionsCursor>
{
    public StakingPositionFilterParametersValidator()
    {
        RuleForEach(filter => filter.LiquidityPools).MustBeNetworkAddress().WithMessage("Liquidity pool must be valid address");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
