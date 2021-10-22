using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using Xunit;

namespace Opdex.Platform.Common.Tests.Extensions
{
    public class Base64ExtensionsTests
    {
        [Fact]
        public void TryBase64Decode_EmptyString_ReturnFalse()
        {
            // Arrange
            var base64Encoded = "";

            // Act
            var canDecode = Base64Extensions.TryBase64Decode(base64Encoded, out var plainText);

            // Assert
            canDecode.Should().Be(false);
            plainText.Should().Be("");
        }

        [Fact]
        public void TryBase64Decode_JustPadding_ReturnFalse()
        {
            // Arrange
            var base64Encoded = "==";

            // Act
            var canDecode = Base64Extensions.TryBase64Decode(base64Encoded, out var plainText);

            // Assert
            canDecode.Should().Be(false);
            plainText.Should().Be("");
        }

        [Fact]
        public void TryBase64Decode_InvalidCharacter_ReturnFalse()
        {
            // Arrange
            var base64Encoded = "@";

            // Act
            var canDecode = Base64Extensions.TryBase64Decode(base64Encoded, out var plainText);

            // Assert
            canDecode.Should().Be(false);
            plainText.Should().Be("");
        }

        [Fact]
        public void TryBase64Decode_ValidData_ReturnTrue()
        {
            // Arrange
            var base64Encoded = "ZGlyZWN0aW9uOkRFU0M7bGltaXQ6MjtwYWdpbmc6Rm9yd2FyZDtpbmNsdWRlWmVyb0Ftb3VudHM6RmFsc2U7cG9pbnRlcjpNdz09Ow==";

            // Act
            var canDecode = Base64Extensions.TryBase64Decode(base64Encoded, out var plainText);

            // Assert
            canDecode.Should().Be(true);
            plainText.Should().Be("direction:DESC;limit:2;paging:Forward;includeZeroAmounts:False;pointer:Mw==;");
        }
    }
}
