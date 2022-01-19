using FluentValidation;
using FluentValidation.Validators;

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
