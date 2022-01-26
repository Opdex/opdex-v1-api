using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;
using Opdex.Platform.WebApi.Validation.MiningPools;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.MiningPools;

public class MiningPoolFilterParametersValidatorTests
{
    private readonly MiningPoolFilterParametersValidator _validator;

    public MiningPoolFilterParametersValidatorTests()
    {
        _validator = new MiningPoolFilterParametersValidator();
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void LiquidityPools_Items_Invalid(Address address)
    {
        // Arrange
        var request = new MiningPoolFilterParameters
        {
            LiquidityPools = new List<Address>() { address }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.LiquidityPools);
    }

    [Fact]
    public void LiquidityPools_Items_Valid()
    {
        // Arrange
        var request = new MiningPoolFilterParameters
        {
            LiquidityPools = new List<Address>() { new Address("tKFkNiL5KJ3Q4br929i6hHbB4X4mt1MigF") }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.LiquidityPools);
    }

    [Fact]
    public void MiningStatus_Invalid()
    {
        // Arrange
        var request = new MiningPoolFilterParameters
        {
            MiningStatus = (MiningStatusFilter)954353
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.MiningStatus);
    }

    [Theory]
    [InlineData(MiningStatusFilter.Any)]
    [InlineData(MiningStatusFilter.Active)]
    [InlineData(MiningStatusFilter.Inactive)]
    public void MiningStatus_Valid(MiningStatusFilter status)
    {
        // Arrange
        var request = new MiningPoolFilterParameters
        {
            MiningStatus = status
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.MiningStatus);
    }

    [Fact]
    public void Limit_Invalid()
    {
        // Arrange
        var request = new MiningPoolFilterParameters
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
        var request = new MiningPoolFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Limit);
    }
}