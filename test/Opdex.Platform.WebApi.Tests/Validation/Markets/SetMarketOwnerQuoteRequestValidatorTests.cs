using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Validation.Markets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Markets;

public class SetMarketOwnerQuoteRequestValidatorTests
{
    private readonly SetMarketOwnerQuoteRequestValidator _validator;

    public SetMarketOwnerQuoteRequestValidatorTests()
    {
        _validator = new SetMarketOwnerQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Owner_Invalid(Address owner)
    {
        // Arrange
        var request = new SetMarketOwnerQuoteRequest
        {
            Owner = owner
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Owner);
    }

    [Fact]
    public void Owner_Valid()
    {
        // Arrange
        var request = new SetMarketOwnerQuoteRequest
        {
            Owner = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Owner);
    }
}
