using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;

namespace Opdex.Platform.WebApi.Validation.MiningPools;

public class MiningPoolFilterParametersValidator : AbstractCursorValidator<MiningPoolFilterParameters, MiningPoolsCursor>
{
    public MiningPoolFilterParametersValidator()
    {
        RuleForEach(filter => filter.LiquidityPools).MustBeNetworkAddress().WithMessage($"Liquidity pool must be valid address.");
        RuleFor(filter => filter.MiningStatus).MustBeValidEnumValue().WithMessage("Mining status filter must be valid for the enumeration values.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
