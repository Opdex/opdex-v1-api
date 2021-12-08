using FluentValidation;
using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Validation;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation;

public class CrsAmountValidatorTests
{
    private readonly StubValidator _validator;

    public CrsAmountValidatorTests()
    {
        _validator = new StubValidator();
    }

    [Fact]
    public void Validate_DefaultFixedDecimal_Invalid()
    {
        // Arrange
        var record = new Stub(default);

        // Act
        var result = _validator.TestValidate(record);

        // Assert
        result.ShouldHaveValidationErrorFor(record => record.Amount);
    }

    [Fact]
    public void Validate_NoDecimalPlaces_Invalid()
    {
        // Arrange
        var record = new Stub(FixedDecimal.Parse("100"));

        // Act
        var result = _validator.TestValidate(record);

        // Assert
        result.ShouldHaveValidationErrorFor(record => record.Amount);
    }

    [Fact]
    public void Validate_SevenDecimalPlaces_Invalid()
    {
        // Arrange
        var record = new Stub(FixedDecimal.Parse("100.0000000"));

        // Act
        var result = _validator.TestValidate(record);

        // Assert
        result.ShouldHaveValidationErrorFor(record => record.Amount);
    }

    [Fact]
    public void Validate_NineDecimalPlaces_Invalid()
    {
        // Arrange
        var record = new Stub(FixedDecimal.Parse("100.000000000"));

        // Act
        var result = _validator.TestValidate(record);

        // Assert
        result.ShouldHaveValidationErrorFor(record => record.Amount);
    }

    [Fact]
    public void Validate_EightDecimalPlacesGreaterThanMaxCrs_Invalid()
    {
        // Arrange
        var record = new Stub(FixedDecimal.Parse("184467440737.09551616"));

        // Act
        var result = _validator.TestValidate(record);

        // Assert
        result.ShouldHaveValidationErrorFor(record => record.Amount);
    }

    [Fact]
    public void Validate_EightDecimalPlacesMaxCrs_Valid()
    {
        // Arrange
        var record = new Stub(FixedDecimal.Parse("184467440737.09551615"));

        // Act
        var result = _validator.TestValidate(record);

        // Assert
        result.ShouldNotHaveValidationErrorFor(record => record.Amount);
    }

    [Fact]
    public void Validate_EightDecimalPlacesZero_Valid()
    {
        // Arrange
        var record = new Stub(FixedDecimal.Parse("0.00000000"));

        // Act
        var result = _validator.TestValidate(record);

        // Assert
        result.ShouldNotHaveValidationErrorFor(record => record.Amount);
    }

    public class StubValidator : AbstractValidator<Stub>
    {
        public StubValidator()
        {
            RuleFor(record => record.Amount).MustBeValidCrsValue();
        }
    }

    public record Stub(FixedDecimal Amount);
}
