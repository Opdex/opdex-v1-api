using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults;

public class RevokeVaultCertificatesQuoteRequestValidatorTests
{
    private readonly RevokeVaultCertificatesQuoteRequestValidator _validator;

    public RevokeVaultCertificatesQuoteRequestValidatorTests()
    {
        _validator = new RevokeVaultCertificatesQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Holder_Invalid(Address holder)
    {
        // Arrange
        var request = new RevokeVaultCertificatesQuoteRequest
        {
            Holder = holder
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Holder);
    }

    [Fact]
    public void Holder_Valid()
    {
        // Arrange
        var request = new RevokeVaultCertificatesQuoteRequest
        {
            Holder = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Holder);
    }
}