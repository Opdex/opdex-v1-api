using FluentValidation;
using FluentValidation.Validators;
using Opdex.Platform.Common.Models;
using ZymLabs.NSwag.FluentValidation;

namespace Opdex.Platform.WebApi.Validation;

public class NetworkAddressValidator<T> : PropertyValidator<T, Address>, INetworkAddressValidator
{
    public override string Name => "NetworkAddress";

    public override bool IsValid(ValidationContext<T> context, Address value)
    {
        return value != Address.Empty && value != Address.Cirrus;
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be valid address.";
}

public class NetworkAddressStringValidator<T> : PropertyValidator<T, string>, INetworkAddressValidator
{
    public override string Name => "NetworkAddress";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        return Address.TryParse(value, out var address) && address != Address.Empty && address != Address.Cirrus.ToString();
    }

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be valid address.";
}

public interface INetworkAddressValidator : IPropertyValidator
{
}

public class NetworkAddressValidationRule : FluentValidationRule
{
    public NetworkAddressValidationRule() : base("NetworkAddress")
    {
        Matches = validator => validator is INetworkAddressValidator;
        Apply = context =>
        {
            var schema = context.SchemaProcessorContext.Schema;
            if (!schema.RequiredProperties.Contains(context.PropertyKey)) schema.RequiredProperties.Add(context.PropertyKey);
            var property = schema.Properties[context.PropertyKey];
            property.IsNullableRaw = false;
            property.MinLength = 30;
            property.MaxLength = 42;
        };
    }
}

public static class NetworkAddressValidatorExtensions
{
    /// <summary>
    /// Validates the value is a network address.
    /// </summary>
    public static IRuleBuilderOptions<T, Address> MustBeNetworkAddress<T>(this IRuleBuilder<T, Address> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new NetworkAddressValidator<T>());
    }

    /// <summary>
    /// Validates the value is a network address.
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeNetworkAddress<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new NetworkAddressStringValidator<T>());
    }
}