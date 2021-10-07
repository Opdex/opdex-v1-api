using FluentValidation;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Validation
{
    public static class AddressRules
    {
        public static IRuleBuilderOptions<T, Address> MustBeSmartContractAddress<T>(this IRuleBuilder<T, Address> ruleBuilder)
        {
            return ruleBuilder.Must(address => address != Address.Empty && address != Address.Cirrus).WithMessage("{PropertyName} must be valid address.");
        }
    }
}
