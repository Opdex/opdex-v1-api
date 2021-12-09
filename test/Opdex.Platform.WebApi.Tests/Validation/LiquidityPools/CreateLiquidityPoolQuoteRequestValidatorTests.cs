using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using Opdex.Platform.WebApi.Validation.LiquidityPools;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.LiquidityPools;

public class CreateLiquidityPoolQuoteRequestValidatorTests
{
    private readonly CreateLiquidityPoolQuoteRequestValidator _validator;

    public CreateLiquidityPoolQuoteRequestValidatorTests()
    {
        _validator = new CreateLiquidityPoolQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Market_Invalid(Address market)
    {
        // Arrange
        var request = new CreateLiquidityPoolQuoteRequest
        {
            Market = market
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Market);
    }

    [Fact]
    public void Market_Valid()
    {
        // Arrange
        var request = new CreateLiquidityPoolQuoteRequest
        {
            Market = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Market);
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Token_Invalid(Address token)
    {
        // Arrange
        var request = new CreateLiquidityPoolQuoteRequest
        {
            Token = token
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Token);
    }

    [Fact]
    public void Token_Valid()
    {
        // Arrange
        var request = new CreateLiquidityPoolQuoteRequest
        {
            Token = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Token);
    }
}
