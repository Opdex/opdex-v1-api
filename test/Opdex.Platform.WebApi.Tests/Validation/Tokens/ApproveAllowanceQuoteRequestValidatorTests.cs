using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Validation.Tokens;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Tokens;

public class ApproveAllowanceQuoteRequestValidatorTests
{
    private readonly ApproveAllowanceQuoteRequestValidator _validator;

    public ApproveAllowanceQuoteRequestValidatorTests()
    {
        _validator = new ApproveAllowanceQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Spender_Invalid(Address spender)
    {
        // Arrange
        var request = new ApproveAllowanceQuoteRequest
        {
            Spender = spender
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Spender);
    }

    [Fact]
    public void Spender_Valid()
    {
        // Arrange
        var request = new ApproveAllowanceQuoteRequest
        {
            Spender = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Spender);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void Amount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new ApproveAllowanceQuoteRequest
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
        var request = new ApproveAllowanceQuoteRequest
        {
            Amount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Amount);
    }
}
