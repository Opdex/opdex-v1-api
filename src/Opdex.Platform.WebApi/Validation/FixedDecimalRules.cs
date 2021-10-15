using FluentValidation;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.WebApi.Validation
{
    public static class FixedDecimalRules
    {
        public static IRuleBuilderOptions<T, FixedDecimal> MustBeValidTokenValue<T>(this IRuleBuilder<T, FixedDecimal> ruleBuilder)
        {
            return ruleBuilder.GreaterThan(FixedDecimal.Zero).MustNotExceedMaxTokenDecimals();
        }

        private static IRuleBuilderOptions<T, FixedDecimal> MustNotExceedMaxTokenDecimals<T>(this IRuleBuilder<T, FixedDecimal> ruleBuilder)
        {
            return ruleBuilder.Must(value => value.Precision <= 18).WithMessage("Tokens can have no more than 18 decimal places.");
        }
    }
}
