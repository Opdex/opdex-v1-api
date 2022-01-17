using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.MarketTokens;
using Opdex.Platform.WebApi.Validation.MarketTokens;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.MarketTokens;

public class SwapAmountInQuoteRequestModelValidatorTests
{
    private readonly SwapAmountInQuoteRequestModelValidator _validator;

    public SwapAmountInQuoteRequestModelValidatorTests()
    {
        _validator = new SwapAmountInQuoteRequestModelValidator();
    }

    [Fact]
    public void TokenOut_Invalid()
    {
        // Arrange
        var request = new SwapAmountInQuoteRequestModel
        {
            TokenOut = Address.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.TokenOut);
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void TokenOut_Valid(Address token)
    {
        // Arrange
        var request = new SwapAmountInQuoteRequestModel
        {
            TokenOut = token
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.TokenOut);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void TokenOutAmount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new SwapAmountInQuoteRequestModel
        {
            TokenOutAmount = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.TokenOutAmount);
    }

    [Fact]
    public void TokenOutAmount_Valid()
    {
        // Arrange
        var request = new SwapAmountInQuoteRequestModel
        {
            TokenOutAmount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.TokenOutAmount);
    }
}
