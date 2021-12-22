using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using Opdex.Platform.WebApi.Validation.LiquidityPools;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.LiquidityPools;

public class RemoveLiquidityQuoteRequestValidatorTests
{
    private readonly RemoveLiquidityQuoteRequestValidator _validator;

    public RemoveLiquidityQuoteRequestValidatorTests()
    {
        _validator = new RemoveLiquidityQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(InvalidCRSAmountData))]
    [ClassData(typeof(ZeroCRSAmountData))]
    public void AmountCrsMin_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new RemoveLiquidityQuoteRequest
        {
            AmountCrsMin = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.AmountCrsMin);
    }

    [Fact]
    public void AmountCrsMin_Valid()
    {
        // Arrange
        var request = new RemoveLiquidityQuoteRequest
        {
            AmountCrsMin = FixedDecimal.Parse("0.00000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.AmountCrsMin);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void AmountSrcMin_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new RemoveLiquidityQuoteRequest
        {
            AmountSrcMin = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.AmountSrcMin);
    }

    [Fact]
    public void AmountSrcMin_Valid()
    {
        // Arrange
        var request = new RemoveLiquidityQuoteRequest
        {
            AmountSrcMin = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.AmountSrcMin);
    }

    [Theory]
    [ClassData(typeof(InvalidOLPTAmountData))]
    [ClassData(typeof(ZeroOLPTAmountData))]
    public void Liquidity_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new RemoveLiquidityQuoteRequest
        {
            Liquidity = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Liquidity);
    }

    [Fact]
    public void Liquidity_Valid()
    {
        // Arrange
        var request = new RemoveLiquidityQuoteRequest
        {
            Liquidity = FixedDecimal.Parse("0.00000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Liquidity);
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Recipient_Invalid(Address recipient)
    {
        // Arrange
        var request = new RemoveLiquidityQuoteRequest
        {
            Recipient = recipient
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Recipient);
    }

    [Fact]
    public void Recipient_Valid()
    {
        // Arrange
        var request = new RemoveLiquidityQuoteRequest
        {
            Recipient = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Recipient);
    }
}
