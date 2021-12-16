using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;
using Opdex.Platform.WebApi.Validation.VaultGovernances;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.VaultGovernances;

public class VaultGovernanceFilterParametersValidatorTests
{
    private readonly VaultGovernanceFilterParametersValidator _validator;

    public VaultGovernanceFilterParametersValidatorTests()
    {
        _validator = new VaultGovernanceFilterParametersValidator();
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    public void LockedToken_Invalid(Address lockedToken)
    {
        // Arrange
        var request = new VaultGovernanceFilterParameters
        {
            LockedToken = lockedToken
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.LockedToken);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void LockedToken_Valid(Address lockedToken)
    {
        // Arrange
        var request = new VaultGovernanceFilterParameters
        {
            LockedToken = lockedToken
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.LockedToken);
    }
}
