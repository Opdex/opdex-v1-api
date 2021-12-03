using FluentValidation;
using FluentValidation.Validators;
using Opdex.Platform.Common.Models;
using ZymLabs.NSwag.FluentValidation;

namespace Opdex.Platform.WebApi.Validation;

public class TokenAmountValidator<T> : PropertyValidator<T, FixedDecimal>, ITokenAmountValidator
{
    public override string Name => "TokenAmount";

    public override bool IsValid(ValidationContext<T> context, FixedDecimal value) => value.Precision <= 18;

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must not contain more than 18 decimal places.";
}

public interface ITokenAmountValidator : IPropertyValidator
{
}

public class TokenAmountValidationRule : FluentValidationRule
{
    public TokenAmountValidationRule() : base("TokenAmount")
    {
        Matches = validator => validator is ITokenAmountValidator;
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

public static class TokenAmountValidatorExtensions
{
    /// <summary>
    /// Validates that the value is a valid token amount.
    /// </summary>
    public static IRuleBuilderOptions<T, FixedDecimal> MustBeValidTokenValue<T>(this IRuleBuilder<T, FixedDecimal> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new TokenAmountValidator<T>());
    }
}