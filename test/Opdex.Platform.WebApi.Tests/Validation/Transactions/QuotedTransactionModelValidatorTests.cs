using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Validation.Transactions;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Transactions;

public class QuotedTransactionModelValidatorTests
{
    private readonly QuotedTransactionModelValidator _validator;

    public QuotedTransactionModelValidatorTests()
    {
        _validator = new QuotedTransactionModelValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Sender_Invalid(Address sender)
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            Sender = sender
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Sender);
    }

    [Fact]
    public void Sender_Valid()
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            Sender = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Sender);
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void To_Invalid(Address to)
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            To = to
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.To);
    }

    [Fact]
    public void To_Valid()
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            To = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.To);
    }

    [Theory]
    [ClassData(typeof(InvalidCRSAmountData))]
    public void Amount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            Amount = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Amount);
    }

    [Fact]
    public void Amount_Valid()
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            Amount = FixedDecimal.Parse("0.00000000")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Amount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Method_Invalid(string method)
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            Method = method
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Method);
    }

    [Fact]
    public void Method_Valid()
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            Method = "Swap"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Method);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Callback_Invalid(string callback)
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            Callback = callback
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Callback);
    }

    [Fact]
    public void Callback_Valid()
    {
        // Arrange
        var request = new QuotedTransactionModel
        {
            Callback = "https://v1-test-api.opdex.com/v1/transactions"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Callback);
    }
}
