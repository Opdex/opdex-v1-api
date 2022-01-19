using FluentValidation;
using FluentValidation.Validators;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Validation;

public class CrsAmountValidator<T> : PropertyValidator<T, FixedDecimal>, ICrsAmountValidator
{
    public override string Name => "CrsAmount";

    public override bool IsValid(ValidationContext<T> context, FixedDecimal value) => value.Precision == 8 && value.ToSatoshis(8) <= ulong.MaxValue;

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must contain exactly 8 decimal places and not exceed max CRS value.";
}

public interface ICrsAmountValidator : IPropertyValidator
{
}

public static class CrsAmountValidatorExtensions
{
    /// <summary>
    /// Validates that the value is a valid CRS amount.
    /// </summary>
    public static IRuleBuilderOptions<T, FixedDecimal> MustBeValidCrsValue<T>(this IRuleBuilder<T, FixedDecimal> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new CrsAmountValidator<T>());
    }
}
