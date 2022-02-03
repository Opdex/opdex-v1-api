using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Transactions;
using Opdex.Platform.WebApi.Validation.Transactions;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Transactions;

public class TransactionBroadcastNotificationValidatorTests
{
    private readonly TransactionBroadcastNotificationValidator _validator;

    public TransactionBroadcastNotificationValidatorTests()
    {
        _validator = new TransactionBroadcastNotificationValidator();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void WalletAddress_Invalid(string publicKey)
    {
        // Arrange
        var request = new TransactionBroadcastNotificationRequest
        {
            TransactionHash = new Sha256(34283925829),
            WalletAddress = publicKey
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.WalletAddress);
    }
}
