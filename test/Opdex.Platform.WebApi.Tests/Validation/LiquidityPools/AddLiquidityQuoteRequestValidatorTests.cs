using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using Opdex.Platform.WebApi.Validation.LiquidityPools;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.LiquidityPools;

public class AddLiquidityQuoteRequestValidatorTests
{
    private readonly AddLiquidityQuoteRequestValidator _validator;

    public AddLiquidityQuoteRequestValidatorTests()
    {
        _validator = new AddLiquidityQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(InvalidCRSAmountData))]
    [ClassData(typeof(ZeroCRSAmountData))]
    public void AmountCrs_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new AddLiquidityQuoteRequest
        {
            AmountCrs = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.AmountCrs);
    }

    [Fact]
    public void AmountCrs_Valid()
    {
        // Arrange
        var request = new AddLiquidityQuoteRequest
        {
            AmountCrs = FixedDecimal.Parse("0.00000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.AmountCrs);
    }

    [Theory]
    [ClassData(typeof(InvalidCRSAmountData))]
    [ClassData(typeof(ZeroCRSAmountData))]
    public void AmountCrsMin_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new AddLiquidityQuoteRequest
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
        var request = new AddLiquidityQuoteRequest
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
    public void AmountSrc_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new AddLiquidityQuoteRequest
        {
            AmountSrc = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.AmountSrc);
    }

    [Fact]
    public void AmountSrc_Valid()
    {
        // Arrange
        var request = new AddLiquidityQuoteRequest
        {
            AmountSrc = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.AmountSrc);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void AmountSrcMin_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new AddLiquidityQuoteRequest
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
        var request = new AddLiquidityQuoteRequest
        {
            AmountSrcMin = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.AmountSrcMin);
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Recipient_Invalid(Address recipient)
    {
        // Arrange
        var request = new AddLiquidityQuoteRequest
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
        var request = new AddLiquidityQuoteRequest
        {
            Recipient = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Recipient);
    }
}
