using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;

namespace Opdex.Platform.WebApi.Validation.MiningPools;

public class MiningPoolFilterParametersValidator : AbstractCursorValidator<MiningPoolFilterParameters, MiningPoolsCursor>
{
    public MiningPoolFilterParametersValidator()
    {
        RuleForEach(filter => filter.LiquidityPools).MustBeNetworkAddress();
        RuleFor(filter => filter.MiningStatus).MustBeValidEnumValue();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}