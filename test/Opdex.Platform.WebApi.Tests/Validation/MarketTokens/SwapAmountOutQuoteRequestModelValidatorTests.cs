using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.MarketTokens;
using Opdex.Platform.WebApi.Validation.MarketTokens;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.MarketTokens;

public class SwapAmountOutQuoteRequestModelValidatorTests
{
    private readonly SwapAmountOutQuoteRequestModelValidator _validator;

    public SwapAmountOutQuoteRequestModelValidatorTests()
    {
        _validator = new SwapAmountOutQuoteRequestModelValidator();
    }

    [Fact]
    public void TokenIn_Invalid()
    {
        // Arrange
        var request = new SwapAmountOutQuoteRequestModel
        {
            TokenIn = Address.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.TokenIn);
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void TokenIn_Valid(Address token)
    {
        // Arrange
        var request = new SwapAmountOutQuoteRequestModel
        {
            TokenIn = token
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.TokenIn);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void TokenInAmount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new SwapAmountOutQuoteRequestModel
        {
            TokenInAmount = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.TokenInAmount);
    }

    [Fact]
    public void TokenInAmount_Valid()
    {
        // Arrange
        var request = new SwapAmountOutQuoteRequestModel
        {
            TokenInAmount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.TokenInAmount);
    }
}
