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
    public void Request_Null_Invalid()
    {
        // Arrange
        var request = new QuoteReplayRequest
        {
            Request = null
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Request);
    }

    [Fact]
    public void Request_NotNull_Valid()
    {
        // Arrange
        var request = new QuoteReplayRequest
        {
            Request = new QuotedTransactionModel()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Request);
    }
}
