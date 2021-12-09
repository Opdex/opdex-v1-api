using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;
using Opdex.Platform.WebApi.Validation.VaultGovernances;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.VaultGovernances;

public class VaultProposalVoteQuoteRequestValidatorTests
{
    private readonly VaultProposalVoteQuoteRequestValidator _validator;

    public VaultProposalVoteQuoteRequestValidatorTests()
    {
        _validator = new VaultProposalVoteQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(InvalidCRSAmountData))]
    [ClassData(typeof(ZeroCRSAmountData))]
    public void Amount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new VaultProposalVoteQuoteRequest
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
        var request = new VaultProposalVoteQuoteRequest
        {
            Amount = FixedDecimal.Parse("0.00000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Amount);
    }
}
