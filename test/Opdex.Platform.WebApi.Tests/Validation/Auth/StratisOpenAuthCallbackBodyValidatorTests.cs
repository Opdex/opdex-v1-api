using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Auth;
using Opdex.Platform.WebApi.Validation.Auth;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Auth;

public class StratisOpenAuthCallbackBodyValidatorTests
{
    private readonly StratisOpenAuthCallbackBodyValidator _validator;

    public StratisOpenAuthCallbackBodyValidatorTests()
    {
        _validator = new StratisOpenAuthCallbackBodyValidator();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("INVALID_SIGNED_MESSAGE_FORMAT")]
    public void Signature_Invalid(string signature)
    {
        // Arrange
        var request = new StratisOpenAuthCallbackBody
        {
            Signature = signature
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Signature);
    }

    [Fact]
    public void Signature_Valid()
    {
        // Arrange
        var request = new StratisOpenAuthCallbackBody
        {
            Signature = "TVlfU0lHTkVEX01FU1NBR0VfTVlfU0lHTkVEX01FU1NBR0VfTVlfU0lHTkVEX01FU1NBR0VfTVlfU0lHTkVEX01FU1NBR0U="
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Signature);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void PublicKey_Invalid(Address publicKey)
    {
        // Arrange
        var request = new StratisOpenAuthCallbackBody
        {
            PublicKey = publicKey
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.PublicKey);
    }

    [Fact]
    public void PublicKey_Valid()
    {
        // Arrange
        var request = new StratisOpenAuthCallbackBody
        {
            PublicKey = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.PublicKey);
    }
}