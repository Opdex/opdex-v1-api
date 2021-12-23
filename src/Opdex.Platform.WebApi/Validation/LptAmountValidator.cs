using FluentValidation;
using FluentValidation.Validators;
using Opdex.Platform.Common.Models;
using ZymLabs.NSwag.FluentValidation;

namespace Opdex.Platform.WebApi.Validation;

public class LptAmountValidator<T> : PropertyValidator<T, FixedDecimal>, ILptAmountValidator
{
    public override string Name => "LptAmount";

    public override bool IsValid(ValidationContext<T> context, FixedDecimal value) => value.Precision == 8;

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must contain exactly 8 decimal places.";
}

public interface ILptAmountValidator : IPropertyValidator
{
}

public class LptAmountValidationRule : FluentValidationRule
{
    public LptAmountValidationRule() : base("LptAmount")
    {
        Matches = validator => validator is ILptAmountValidator;
        Apply = context =>
        {
            var schema = context.SchemaProcessorContext.Schema;
            if (!schema.RequiredProperties.Contains(context.PropertyKey)) schema.RequiredProperties.Add(context.PropertyKey);
            var property = schema.Properties[context.PropertyKey];
            property.Pattern = @"^\d*\.\d{8}$";
        };
    }
}

public static class LptAmountValidatorExtensions
{
    /// <summary>
    /// Validates that the value is a valid OLPT amount.
    /// </summary>
    public static IRuleBuilderOptions<T, FixedDecimal> MustBeValidLptValue<T>(this IRuleBuilder<T, FixedDecimal> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new LptAmountValidator<T>());
    }
}
