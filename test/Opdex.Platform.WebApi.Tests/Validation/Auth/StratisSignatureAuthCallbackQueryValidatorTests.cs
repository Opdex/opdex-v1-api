using FluentValidation.TestHelper;
using Opdex.Platform.WebApi.Validation.Auth;
using SSAS.NET;
using System;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Auth;

public class StratisSignatureAuthCallbackQueryValidatorTests
{
    private readonly StratisSignatureAuthCallbackQueryValidator _validator;

    public StratisSignatureAuthCallbackQueryValidatorTests()
    {
        _validator = new StratisSignatureAuthCallbackQueryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Uid_Invalid(string uid)
    {
        // Arrange
        var request = new StratisSignatureAuthCallbackQuery
        {
            Uid = uid
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Uid);
    }

    [Fact]
    public void Uid_Valid()
    {
        // Arrange
        var request = new StratisSignatureAuthCallbackQuery
        {
            Uid = Guid.NewGuid().ToString()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Uid);
    }

    [Theory]
    [InlineData(-62135596801)]
    [InlineData(0)]
    [InlineData(273402300800)]
    public void Exp_Invalid(long exp)
    {
        // Arrange
        var request = new StratisSignatureAuthCallbackQuery
        {
            Exp = exp
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Exp);
    }

    [Fact]
    public void Exp_Valid()
    {
        // Arrange
        var request = new StratisSignatureAuthCallbackQuery
        {
            Exp = 1777777777
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Exp);
    }
}