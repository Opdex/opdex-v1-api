using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using Opdex.Platform.WebApi.Validation.LiquidityPools;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.LiquidityPools;

public class StopStakingQuoteRequestValidatorTests
{
    private readonly StopStakingQuoteRequestValidator _validator;

    public StopStakingQuoteRequestValidatorTests()
    {
        _validator = new StopStakingQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void Amount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new StopStakingQuoteRequest
        {
            Amount = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Amount);
    }

    [Fact]
    public void Amount_Valid()
    {
        // Arrange
        var request = new StopStakingQuoteRequest
        {
            Amount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Amount);
    }
}
