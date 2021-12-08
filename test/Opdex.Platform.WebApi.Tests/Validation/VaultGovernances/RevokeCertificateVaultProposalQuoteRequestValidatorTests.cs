using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;
using Opdex.Platform.WebApi.Validation.VaultGovernances;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.VaultGovernances;

public class RevokeCertificateVaultProposalQuoteRequestValidatorTests
{
    private readonly RevokeCertificateVaultProposalQuoteRequestValidator _validator;

    public RevokeCertificateVaultProposalQuoteRequestValidatorTests()
    {
        _validator = new RevokeCertificateVaultProposalQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Owner_Invalid(Address owner)
    {
        // Arrange
        var request = new RevokeCertificateVaultProposalQuoteRequest
        {
            Owner = owner
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Owner);
    }

    [Fact]
    public void Owner_Valid()
    {
        // Arrange
        var request = new RevokeCertificateVaultProposalQuoteRequest
        {
            Owner = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Owner);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespaceStringData))]
    [InlineData("201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHA")]
    public void Description_Invalid(string description)
    {
        // Arrange
        var request = new RevokeCertificateVaultProposalQuoteRequest
        {
            Description = description
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Description);
    }

    [Fact]
    public void Description_Valid()
    {
        // Arrange
        var request = new RevokeCertificateVaultProposalQuoteRequest
        {
            Description = "200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CH"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Description);
    }
}
