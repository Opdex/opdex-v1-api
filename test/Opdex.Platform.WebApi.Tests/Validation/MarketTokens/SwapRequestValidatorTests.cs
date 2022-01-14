using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.MarketTokens;
using Opdex.Platform.WebApi.Validation.MarketTokens;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.MarketTokens;

public class SwapRequestValidatorTests
{
    private readonly SwapRequestValidator _validator;

    public SwapRequestValidatorTests()
    {
        _validator = new SwapRequestValidator();
    }

    [Fact]
    public void TokenOut_Invalid()
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenOut = Address.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TokenOut);
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void TokenOut_Valid(Address token)
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenOut = token
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.TokenOut);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void TokenInAmount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenInAmount = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TokenInAmount);
    }

    [Fact]
    public void TokenInAmount_Valid()
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenInAmount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.TokenInAmount);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void TokenOutAmount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenOutAmount = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TokenOutAmount);
    }

    [Fact]
    public void TokenOutAmount_Valid()
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenOutAmount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.TokenOutAmount);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    [ClassData(typeof(FiveHundredCRSAmountData))]
    public void TokenInMaximumAmount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenInAmount = FixedDecimal.Parse("500.00000001"),
            TokenInMaximumAmount = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TokenInMaximumAmount);
    }

    [Fact]
    public void TokenInMaximumAmount_Valid()
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenInAmount = FixedDecimal.Parse("0.000000000000000001"),
            TokenInMaximumAmount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.TokenInMaximumAmount);
    }

    [Fact]
    public void TokenInMaximumAmount_Exact_Invalid()
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenInAmount = FixedDecimal.Parse("500.00000000"),
            TokenInMaximumAmount = FixedDecimal.Parse("505.00000000"),
            TokenInExactAmount = true
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TokenInMaximumAmount);
    }

    [Fact]
    public void TokenInMaximumAmount_Exact_Valid()
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenInAmount = FixedDecimal.Parse("505.00000000"),
            TokenInMaximumAmount = FixedDecimal.Parse("505.00000000"),
            TokenInExactAmount = true
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.TokenInMaximumAmount);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    [ClassData(typeof(FiveHundredCRSAmountData))]
    public void TokenOutMinimumAmount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenOutAmount = FixedDecimal.Parse("499.99999999"),
            TokenOutMinimumAmount = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TokenOutMinimumAmount);
    }

    [Fact]
    public void TokenOutMinimumAmount_Valid()
    {
        // Arrange
        var request = new SwapRequest
        {
            TokenOutAmount = FixedDecimal.Parse("0.000000000000000001"),
            TokenOutMinimumAmount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.TokenOutMinimumAmount);
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Recipient_Invalid(Address address)
    {
        // Arrange
        var request = new SwapRequest
        {
            Recipient = address
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Recipient);
    }

    [Theory]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void Recipient_Valid(Address address)
    {
        // Arrange
        var request = new SwapRequest
        {
            Recipient = address
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Recipient);
    }
}
