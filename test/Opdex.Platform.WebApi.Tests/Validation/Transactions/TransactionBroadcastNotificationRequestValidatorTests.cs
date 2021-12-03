using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Transactions;
using Opdex.Platform.WebApi.Validation.Transactions;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Transactions;

public class TransactionBroadcastNotificationRequestValidatorTests
{
    private readonly TransactionBroadcastNotificationRequestValidator _validator;

    public TransactionBroadcastNotificationRequestValidatorTests()
    {
        _validator = new TransactionBroadcastNotificationRequestValidator();
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void WalletAddress_Invalid(Address wallet)
    {
        // Arrange
        var request = new TransactionBroadcastNotificationRequest
        {
            WalletAddress = wallet
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.WalletAddress);
    }

    [Theory]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void WalletAddress_Valid(Address wallet)
    {
        // Arrange
        var request = new TransactionBroadcastNotificationRequest
        {
            WalletAddress = wallet
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.WalletAddress);
    }
}