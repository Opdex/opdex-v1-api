using FluentValidation;
using FluentValidation.Validators;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Validation;

public class NetworkAddressOrCrsValidator<T> : PropertyValidator<T, Address>, INetworkAddressOrCrsValidator
{
    public override string Name => "NetworkAddressOrCrs";

    public override bool IsValid(ValidationContext<T> context, Address value) => value != Address.Empty;

    protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be valid address or CRS.";
}

public interface INetworkAddressOrCrsValidator : IPropertyValidator
{
}

public static class NetworkAddressOrCrsValidatorExtensions
{
    /// <summary>
    /// Validates the value is CRS or a network address.
    /// </summary>
    public static IRuleBuilderOptions<T, Address> MustBeNetworkAddressOrCrs<T>(this IRuleBuilder<T, Address> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new NetworkAddressOrCrsValidator<T>());
    }
}