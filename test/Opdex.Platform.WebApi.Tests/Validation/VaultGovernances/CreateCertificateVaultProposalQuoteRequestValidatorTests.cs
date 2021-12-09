using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;
using Opdex.Platform.WebApi.Validation.VaultGovernances;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.VaultGovernances;

public class CreateCertificateVaultProposalQuoteRequestValidatorTests
{
    private readonly CreateCertificateVaultProposalQuoteRequestValidator _validator;

    public CreateCertificateVaultProposalQuoteRequestValidatorTests()
    {
        _validator = new CreateCertificateVaultProposalQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Owner_Invalid(Address owner)
    {
        // Arrange
        var request = new CreateCertificateVaultProposalQuoteRequest
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
        var request = new CreateCertificateVaultProposalQuoteRequest
        {
            Owner = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Owner);
    }

    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void Amount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new CreateCertificateVaultProposalQuoteRequest
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
        var request = new CreateCertificateVaultProposalQuoteRequest
        {
            Amount = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Amount);
    }

    [Theory]
    [ClassData(typeof(NullOrWhitespaceStringData))]
    [InlineData("201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHARACTERS201CHA")]
    public void Description_Invalid(string description)
    {
        // Arrange
        var request = new CreateCertificateVaultProposalQuoteRequest
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
        var request = new CreateCertificateVaultProposalQuoteRequest
        {
            Description = "200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CH"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Description);
    }
}
