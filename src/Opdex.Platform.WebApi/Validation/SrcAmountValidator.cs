using FluentValidation;
using FluentValidation.Validators;
using Opdex.Platform.Common.Models;
using ZymLabs.NSwag.FluentValidation;

namespace Opdex.Platform.WebApi.Validation;

public class SrcAmountValidator<T> : PropertyValidator<T, FixedDecimal>, ISrcAmountValidator
{
    public override string Name => "SrcAmount";

    public override bool IsValid(ValidationContext<T> context, FixedDecimal value) => value.Precision <= 18;

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must not contain more than 18 decimal places.";
}

public interface ISrcAmountValidator : IPropertyValidator
{
}

public class SrcAmountValidationRule : FluentValidationRule
{
    public SrcAmountValidationRule() : base("SrcAmount")
    {
        Matches = validator => validator is ISrcAmountValidator;
        Apply = context =>
        {
            var schema = context.SchemaProcessorContext.Schema;
            if (!schema.RequiredProperties.Contains(context.PropertyKey)) schema.RequiredProperties.Add(context.PropertyKey);
            var property = schema.Properties[context.PropertyKey];
            property.Pattern = @"^\d*\.\d{1,18}$";
            property.IsNullableRaw = false;
        };
    }
}

public static class SrcAmountValidatorExtensions
{
    /// <summary>
    /// Validates that the value is a valid token amount.
    /// </summary>
    public static IRuleBuilderOptions<T, FixedDecimal> MustBeValidSrcValue<T>(this IRuleBuilder<T, FixedDecimal> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new SrcAmountValidator<T>());
    }
}