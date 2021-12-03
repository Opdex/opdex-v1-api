using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.MiningPools;
using Opdex.Platform.WebApi.Validation.MiningPools;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.MiningPools;

public class MiningQuoteValidatorTests
{
    private readonly MiningQuoteValidator _validator;

    public MiningQuoteValidatorTests()
    {
        _validator = new MiningQuoteValidator();
    }

    [Fact]
    public void Amount_Zero_Invalid()
    {
        // Arrange
        var request = new MiningQuote
        {
            Amount = FixedDecimal.Zero,
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }

    [Fact]
    public void Amount_GreaterThanZero_Valid()
    {
        // Arrange
        var request = new MiningQuote
        {
            Amount = FixedDecimal.Parse("0.00000001"),
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Amount);
    }
}