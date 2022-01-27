using FluentValidation.TestHelper;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Validation.Transactions;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Transactions;

public class TransactionParameterModelValidatorTests
{
    private readonly TransactionParameterModelValidator _validator;

    public TransactionParameterModelValidatorTests()
    {
        _validator = new TransactionParameterModelValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Label_Invalid(string label)
    {
        // Arrange
        var request = new TransactionParameterModel
        {
            Label = label
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Label);
    }

    [Fact]
    public void Label_Valid()
    {
        // Arrange
        var request = new TransactionParameterModel
        {
            Label = "Sender"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Label);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0#InvalidParameterType")]
    [InlineData("13#InvalidParameterType")]
    [InlineData("1#")]
    [InlineData("1:InvalidSeparator")]
    public void Value_Invalid(string value)
    {
        // Arrange
        var request = new TransactionParameterModel
        {
            Value = value
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Value);
    }

    [Fact]
    public void Value_Valid()
    {
        // Arrange
        var request = new TransactionParameterModel
        {
            Value = "1#True"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Value);
    }
}
