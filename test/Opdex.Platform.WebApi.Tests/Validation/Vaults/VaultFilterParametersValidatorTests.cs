using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults;

public class VaultFilterParametersValidatorTests
{
    private readonly VaultFilterParametersValidator _validator;

    public VaultFilterParametersValidatorTests()
    {
        _validator = new VaultFilterParametersValidator();
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    public void LockedToken_Invalid(Address lockedToken)
    {
        // Arrange
        var request = new VaultFilterParameters
        {
            LockedToken = lockedToken
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.LockedToken);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void LockedToken_Valid(Address lockedToken)
    {
        // Arrange
        var request = new VaultFilterParameters
        {
            LockedToken = lockedToken
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.LockedToken);
    }

    [Fact]
    public void Limit_Invalid()
    {
        // Arrange
        var request = new VaultFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit + 1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Limit);
    }

    [Fact]
    public void Limit_Valid()
    {
        // Arrange
        var request = new VaultFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Limit);
    }
}