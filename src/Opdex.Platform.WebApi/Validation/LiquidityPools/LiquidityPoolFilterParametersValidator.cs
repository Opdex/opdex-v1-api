using FluentValidation;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using System.Text.RegularExpressions;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools;

public class LiquidityPoolFilterParametersValidator : AbstractCursorValidator<LiquidityPoolFilterParameters, LiquidityPoolsCursor>
{
    private static readonly Regex Alphanumeric = new("^[0-9A-Za-z ]+$", RegexOptions.Compiled);

    public LiquidityPoolFilterParametersValidator()
    {
        RuleFor(filter => filter.Keyword)
            .Must(keyword => !keyword.HasValue() || Alphanumeric.IsMatch(keyword))
            .WithMessage("Keyword must consist of letters, numbers and spaces only.");

        RuleFor(filter => filter.OrderBy).MustBeValidEnumValue();
        RuleFor(filter => filter.StakingFilter).MustBeValidEnumValue();
        RuleFor(filter => filter.MiningFilter).MustBeValidEnumValue();
        RuleFor(filter => filter.NominationFilter).MustBeValidEnumValue();
        RuleForEach(filter => filter.Markets).MustBeNetworkAddress();
        RuleForEach(filter => filter.LiquidityPools).MustBeNetworkAddress();
        RuleForEach(filter => filter.Tokens).MustBeNetworkAddress();
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit);
    }
}