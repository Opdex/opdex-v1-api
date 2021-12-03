using FluentValidation.TestHelper;
using Opdex.Platform.WebApi.Models.Requests.Auth;
using Opdex.Platform.WebApi.Validation.Auth;
using System;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Auth;

public class StratisOpenAuthCallbackQueryValidatorTests
{
    private readonly StratisOpenAuthCallbackQueryValidator _validator;

    public StratisOpenAuthCallbackQueryValidatorTests()
    {
        _validator = new StratisOpenAuthCallbackQueryValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Uid_Invalid(string uid)
    {
        // Arrange
        var request = new StratisOpenAuthCallbackQuery
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
        var request = new StratisOpenAuthCallbackQuery
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
        var request = new StratisOpenAuthCallbackQuery
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
        var request = new StratisOpenAuthCallbackQuery
        {
            Exp = 1777777777
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Exp);
    }
}