using FluentValidation.TestHelper;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Transactions;
using Opdex.Platform.WebApi.Validation.Transactions;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Transactions;

public class QuoteReplayRequestValidatorTests
{
    private readonly QuoteReplayRequestValidator _validator;

    public QuoteReplayRequestValidatorTests()
    {
        _validator = new QuoteReplayRequestValidator();
    }

    [Fact]
    public void Quote_Null_Invalid()
    {
        // Arrange
        var request = new QuoteReplayRequest
        {
            Quote = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Quote);
    }

    [Fact]
    public void Quote_NotNull_Valid()
    {
        // Arrange
        var request = new QuoteReplayRequest
        {
            Quote = new QuotedTransactionModel()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Quote);
    }
}
