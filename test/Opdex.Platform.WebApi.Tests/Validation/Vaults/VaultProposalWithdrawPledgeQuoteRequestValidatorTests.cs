using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults;

public class VaultProposalWithdrawPledgeQuoteRequestValidatorTests
{
    private readonly VaultProposalWithdrawPledgeQuoteRequestValidator _validator;

    public VaultProposalWithdrawPledgeQuoteRequestValidatorTests()
    {
        _validator = new VaultProposalWithdrawPledgeQuoteRequestValidator();
    }

    [Theory]
    [ClassData(typeof(InvalidCRSAmountData))]
    [ClassData(typeof(ZeroCRSAmountData))]
    public void Amount_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new VaultProposalWithdrawPledgeQuoteRequest
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
        var request = new VaultProposalWithdrawPledgeQuoteRequest
        {
            Amount = FixedDecimal.Parse("0.00000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Amount);
    }
}
