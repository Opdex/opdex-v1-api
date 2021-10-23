using FluentValidation.TestHelper;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Validation.Transactions;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Transactions
{
    public class QuoteReplayRequestValidatorTests
    {
        private readonly QuoteReplayRequestValidator _validator;

        public QuoteReplayRequestValidatorTests()
        {
            _validator = new QuoteReplayRequestValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("@%$£$£()*d")]
        [InlineData("==")]
        public void Quote_Invalid(string quote)
        {
            // Arrange
            var request = new QuoteReplayRequest
            {
                Quote = quote
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Quote);
        }

        [Theory]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/")]
        [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/AB==")]
        public void Quote_Valid(string quote)
        {
            // Arrange
            var request = new QuoteReplayRequest
            {
                Quote = quote
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.Quote);
        }
    }
}
