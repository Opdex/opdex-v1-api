using FluentValidation;
using FluentValidation.Validators;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.WebApi.Validation;

public class Base64StringValidator<T> : PropertyValidator<T, string>, IBase64StringValidator
{
    public override string Name => "Base64";

    public override bool IsValid(ValidationContext<T> context, string value) => Base64Extensions.TryBase64Decode(value, out _);

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be base-64 encoded string.";
}

public interface IBase64StringValidator : IPropertyValidator
{
}

public static class Base64StringValidatorExtensions
{
    /// <summary>
    /// Validates the value is a base-64 encoded string.
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeBase64Encoded<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new Base64StringValidator<T>());
    }
}
