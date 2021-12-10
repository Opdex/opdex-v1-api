using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using Opdex.Platform.WebApi.Validation.LiquidityPools;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.LiquidityPools;

public class SkimQuoteRequestValidatorTests
{
    private readonly SkimQuoteRequestValidator _validator;

    public SkimQuoteRequestValidatorTests()
    {
        _validator = new SkimQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Recipient_Invalid(Address recipient)
    {
        // Arrange
        var request = new SkimQuoteRequest
        {
            Recipient = recipient
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Recipient);
    }

    [Fact]
    public void Recipient_Valid()
    {
        // Arrange
        var request = new SkimQuoteRequest
        {
            Recipient = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Recipient);
    }
}
