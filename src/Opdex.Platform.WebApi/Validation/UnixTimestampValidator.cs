using FluentValidation;
using FluentValidation.Validators;
using ZymLabs.NSwag.FluentValidation;

namespace Opdex.Platform.WebApi.Validation;

public class UnixTimestampValidator<T> : PropertyValidator<T, long>, IUnixTimestampValidator
{
    public override string Name => "UnixTimestamp";

    public override bool IsValid(ValidationContext<T> context, long value) => value is > 0 and < 273402300800;

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be unix timestamp.";
}

public interface IUnixTimestampValidator : IPropertyValidator
{
}

public class UnixTimestampValidationRule : FluentValidationRule
{
    public UnixTimestampValidationRule() : base("UnixTimestamp")
    {
        Matches = validator => validator is IUnixTimestampValidator;
        Apply = context =>
        {
            var schema = context.SchemaProcessorContext.Schema;
            if (!schema.RequiredProperties.Contains(context.PropertyKey)) schema.RequiredProperties.Add(context.PropertyKey);
            var property = schema.Properties[context.PropertyKey];
            property.Minimum = 0;
            property.Maximum = 273402300800;
        };
    }
}

public static class UnixTimestampValidatorExtensions
{
    /// <summary>
    /// Validates that the value is a unix timestamp
    /// </summary>
    public static IRuleBuilderOptions<T, long> MustBeUnixTimestamp<T>(this IRuleBuilder<T, long> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new UnixTimestampValidator<T>());
    }
}
