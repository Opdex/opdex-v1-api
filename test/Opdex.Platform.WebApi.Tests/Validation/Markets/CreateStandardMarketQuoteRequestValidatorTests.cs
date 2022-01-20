using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Validation.Markets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Markets;

public class CreateStandardMarketQuoteRequestValidatorTests
{
    private readonly CreateStandardMarketQuoteRequestValidator _validator;

    public CreateStandardMarketQuoteRequestValidatorTests()
    {
        _validator = new CreateStandardMarketQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Owner_Invalid(Address owner)
    {
        // Arrange
        var request = new CreateStandardMarketQuoteRequest
        {
            Owner = owner
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Owner);
    }

    [Fact]
    public void Owner_Valid()
    {
        // Arrange
        var request = new CreateStandardMarketQuoteRequest
        {
            Owner = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Owner);
    }

    [Theory]
    [InlineData("-0.1")]
    [InlineData("0.51")]
    [InlineData("1.1")]
    public void TransactionFeePercent_Invalid(string value)
    {
        // Arrange
        var fee = decimal.Parse(value);
        var request = new CreateStandardMarketQuoteRequest
        {
            TransactionFeePercent = fee
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TransactionFeePercent);
    }

    [Fact]
    public void EnableMarketFee_True_ZeroFeeInvalid()
    {
        // Arrange
        var request = new CreateStandardMarketQuoteRequest
        {
            TransactionFeePercent = 0m,
            EnableMarketFee = true
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.EnableMarketFee);
    }

    [Fact]
    public void EnableMarketFee_False_ZeroFeeValid()
    {
        // Arrange
        var request = new CreateStandardMarketQuoteRequest
        {
            TransactionFeePercent = 0m,
            EnableMarketFee = false
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.EnableMarketFee);
    }

    [Theory]
    [InlineData("0.1")]
    [InlineData("0.2")]
    [InlineData("0.3")]
    [InlineData("0.4")]
    [InlineData("0.5")]
    [InlineData("0.6")]
    [InlineData("0.7")]
    [InlineData("0.8")]
    [InlineData("0.9")]
    [InlineData("1.0")]
    public void TransactionFeePercent_Valid(string value)
    {
        // Arrange
        var fee = decimal.Parse(value);
        var request = new CreateStandardMarketQuoteRequest
        {
            TransactionFeePercent = fee
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.TransactionFeePercent);
    }
}
