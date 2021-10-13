using FluentValidation;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Validation
{
    public static class AddressRules
    {
        public static IRuleBuilderOptions<T, Address> MustBeEmptyAddress<T>(this IRuleBuilder<T, Address> ruleBuilder)
        {
            return ruleBuilder.Must(address => address == Address.Empty).WithMessage("{PropertyName} must be empty.");
        }

        public static IRuleBuilderOptions<T, Address> MustBeNetworkAddress<T>(this IRuleBuilder<T, Address> ruleBuilder)
        {
            return ruleBuilder.Must(address => address != Address.Empty && address != Address.Cirrus).WithMessage("{PropertyName} must be valid address.");
        }

        public static IRuleBuilderOptions<T, Address> MustBeNetworkAddressOrEmpty<T>(this IRuleBuilder<T, Address> ruleBuilder)
        {
            return ruleBuilder.Must(address => address != Address.Cirrus).WithMessage("{PropertyName} must be valid address if provided.");
        }
    }
}
