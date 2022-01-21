using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Validation.Markets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Markets;

public class CollectMarketFeesQuoteRequestValidatorTests
{
    private readonly CollectMarketFeesQuoteRequestValidator _validator;

    public CollectMarketFeesQuoteRequestValidatorTests()
    {
        _validator = new CollectMarketFeesQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Token_Invalid(Address stakingToken)
    {
        // Arrange
        var request = new CollectMarketFeesQuoteRequest
        {
            Token = stakingToken
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Token);
    }

    [Fact]
    public void Token_Valid()
    {
        // Arrange
        var request = new CollectMarketFeesQuoteRequest
        {
            Token = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Token);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void Amount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new CollectMarketFeesQuoteRequest
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
        var request = new CollectMarketFeesQuoteRequest
        {
            Amount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Amount);
    }
}
