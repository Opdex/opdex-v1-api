using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Validation.Markets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Markets;

public class CreateStakingMarketQuoteRequestValidatorTests
{
    private readonly CreateStakingMarketQuoteRequestValidator _validator;

    public CreateStakingMarketQuoteRequestValidatorTests()
    {
        _validator = new CreateStakingMarketQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void StakingToken_Invalid(Address stakingToken)
    {
        // Arrange
        var request = new CreateStakingMarketQuoteRequest
        {
            StakingToken = stakingToken
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.StakingToken);
    }

    [Fact]
    public void StakingToken_Valid()
    {
        // Arrange
        var request = new CreateStakingMarketQuoteRequest
        {
            StakingToken = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.StakingToken);
    }
}
