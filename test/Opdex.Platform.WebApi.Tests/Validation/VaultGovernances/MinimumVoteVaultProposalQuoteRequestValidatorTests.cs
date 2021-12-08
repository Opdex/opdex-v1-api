using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;
using Opdex.Platform.WebApi.Validation.VaultGovernances;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.VaultGovernances;

public class MinimumVoteVaultProposalQuoteRequestValidatorTests
{
    private readonly MinimumVoteVaultProposalQuoteRequestValidator _validator;

    public MinimumVoteVaultProposalQuoteRequestValidatorTests()
    {
        _validator = new MinimumVoteVaultProposalQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(InvalidCRSAmountData))]
    [ClassData(typeof(ZeroCRSAmountData))]
    public void Amount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new MinimumVoteVaultProposalQuoteRequest
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
        var request = new MinimumVoteVaultProposalQuoteRequest
        {
            Amount = FixedDecimal.Parse("0.00000001")
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
        var request = new MinimumVoteVaultProposalQuoteRequest
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
        var request = new MinimumVoteVaultProposalQuoteRequest
        {
            Description = "200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CHARACTERS200CH"
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Description);
    }
}
