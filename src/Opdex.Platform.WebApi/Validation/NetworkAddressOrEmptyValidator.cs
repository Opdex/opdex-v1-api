using FluentValidation;
using FluentValidation.Validators;
using Opdex.Platform.Common.Models;
using ZymLabs.NSwag.FluentValidation;

namespace Opdex.Platform.WebApi.Validation
{
    public class NetworkAddressOrEmptyValidator<T> : PropertyValidator<T, Address>, INetworkAddressOrEmptyValidator
    {
        public override string Name => "NetworkAddressOrEmpty";

        public override bool IsValid(ValidationContext<T> context, Address value)
        {
            return value != Address.Cirrus;
        }

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be valid address if provided.";
    }

    public interface INetworkAddressOrEmptyValidator : IPropertyValidator
    {
    }

    public class NetworkAddressOrEmptyValidationRule : FluentValidationRule
    {
        public NetworkAddressOrEmptyValidationRule() : base("NetworkAddressOrEmpty")
        {
            Matches = validator => validator is INetworkAddressValidator;
            Apply = context =>
            {
                var schema = context.SchemaProcessorContext.Schema;
                if (!schema.RequiredProperties.Contains(context.PropertyKey)) schema.RequiredProperties.Add(context.PropertyKey);
                var property = schema.Properties[context.PropertyKey];
                property.IsNullableRaw = true;
                property.MinLength = 30;
                property.MaxLength = 42;
            };
        }
    }

    public static class NetworkAddressOrEmptyValidatorExtensions
    {
        /// <summary>
        /// Validates the value is an empty address or a network address.
        /// </summary>
        public static IRuleBuilderOptions<T, Address> MustBeNetworkAddressOrEmpty<T>(this IRuleBuilder<T, Address> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new NetworkAddressOrEmptyValidator<T>());
        }
    }
}
