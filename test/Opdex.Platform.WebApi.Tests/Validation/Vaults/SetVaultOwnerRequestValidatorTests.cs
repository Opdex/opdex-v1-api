using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults;

public class SetVaultOwnerQuoteRequestValidatorTests
{
    private readonly SetVaultOwnerQuoteRequestValidator _validator;

    public SetVaultOwnerQuoteRequestValidatorTests()
    {
        _validator = new SetVaultOwnerQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Owner_Invalid(Address owner)
    {
        // Arrange
        var request = new SetVaultOwnerQuoteRequest
        {
            Owner = owner
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Owner);
    }

    [Fact]
    public void Owner_Valid()
    {
        // Arrange
        var request = new SetVaultOwnerQuoteRequest
        {
            Owner = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Owner);
    }
}