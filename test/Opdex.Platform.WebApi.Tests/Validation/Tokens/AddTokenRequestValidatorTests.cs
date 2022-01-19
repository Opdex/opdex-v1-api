using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Validation.Tokens;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Tokens;

public class AddTokenRequestValidatorTests
{
    private readonly AddTokenRequestValidator _validator;

    public AddTokenRequestValidatorTests()
    {
        _validator = new AddTokenRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void TokenAddress_Invalid(Address token)
    {
        // Arrange
        var request = new AddTokenRequest
        {
            Token = token
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Token);
    }

    [Fact]
    public void TokenAddress_Valid()
    {
        // Arrange
        var request = new AddTokenRequest
        {
            Token = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Token);
    }
}
