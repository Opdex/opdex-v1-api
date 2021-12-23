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

    [Theory]
    [ClassData(typeof(InvalidOLPTAmountData))]
    [ClassData(typeof(ZeroOLPTAmountData))]
    public void Amount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new MiningQuote
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
        var request = new MiningQuote
        {
            Amount = FixedDecimal.Parse("0.00000001"),
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Amount);
    }
}
