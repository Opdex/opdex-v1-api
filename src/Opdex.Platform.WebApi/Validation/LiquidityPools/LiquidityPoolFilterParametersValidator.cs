using FluentValidation;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using System.Text.RegularExpressions;

namespace Opdex.Platform.WebApi.Validation.LiquidityPools
{
    public class LiquidityPoolFilterParametersTests : AbstractCursorValidator<LiquidityPoolFilterParameters, LiquidityPoolsCursor>
    {
        private static readonly Regex Regex = new Regex("^[0-9A-Za-z ]+$");

        public LiquidityPoolFilterParametersTests()
        {
            RuleFor(filter => filter.Keyword)
                .Must(keyword => !keyword.HasValue() || Regex.IsMatch(keyword))
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
}
