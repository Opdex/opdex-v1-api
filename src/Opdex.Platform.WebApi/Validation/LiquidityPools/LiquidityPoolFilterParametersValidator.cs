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

        RuleFor(filter => filter.OrderBy).MustBeValidEnumValue().WithMessage("Order must be valid for the enumeration values.");
        RuleFor(filter => filter.StakingStatus).MustBeValidEnumValue().WithMessage("Staking status must be valid for the enumeration values.");
        RuleFor(filter => filter.MiningStatus).MustBeValidEnumValue().WithMessage("Mining status must be valid for the enumeration values.");
        RuleFor(filter => filter.NominationStatus).MustBeValidEnumValue().WithMessage("Nomination status must be valid for the enumeration values.");
        RuleForEach(filter => filter.Markets).MustBeNetworkAddress().WithMessage("Market must be valid address.");
        RuleForEach(filter => filter.LiquidityPools).MustBeNetworkAddress().WithMessage("Liquidity pool must be valid address.");
        RuleForEach(filter => filter.Tokens).MustBeNetworkAddress().WithMessage("Token must be valid address.");
        RuleFor(filter => filter.Limit).LessThanOrEqualTo(Cursor.DefaultMaxLimit).WithMessage($"Limit must be between 1 and {Cursor.DefaultMaxLimit}.");
    }
}
