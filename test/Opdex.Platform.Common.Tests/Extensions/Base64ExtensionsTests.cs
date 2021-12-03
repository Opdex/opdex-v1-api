using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using System.Text;
using Xunit;

namespace Opdex.Platform.Common.Tests.Extensions;

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

    [Fact]
    public void UrlSafeBase64Encode_AdditionSign_SwapToDash()
    {
        // Arrange
        var plainText = "14<+_~`/7'88";

        // Act
        var encoded = Base64Extensions.UrlSafeBase64Encode(Encoding.UTF8.GetBytes(plainText));

        // Assert
        encoded.Should().Be("MTQ8K19-YC83Jzg4");
    }

    [Fact]
    public void UrlSafeBase64Encode_ForwardSlashSign_SwapToUnderscore()
    {
        // Arrange
        var plainText = "ZSWEDXCFRTGVBHYUJNMKIOL<>:P{\"?|}+_)(*&^%$£@!";

        // Act
        var encoded = Base64Extensions.UrlSafeBase64Encode(Encoding.UTF8.GetBytes(plainText));

        // Assert
        encoded.Should().Be("WlNXRURYQ0ZSVEdWQkhZVUpOTUtJT0w8PjpQeyI_fH0rXykoKiZeJSTCo0Ah");
    }

    [Fact]
    public void UrlSafeBase64Encode_Padding_Remove()
    {
        // Arrange
        var plainText = "1234567890";

        // Act
        var encoded = Base64Extensions.UrlSafeBase64Encode(Encoding.UTF8.GetBytes(plainText));

        // Assert
        encoded.Should().Be("MTIzNDU2Nzg5MA");
    }

    [Fact]
    public void UrlSafeBase64_DashSign_Recognise()
    {
        // Arrange
        var encoded = "MTQ8K19-YC83Jzg4";

        // Act
        var plainText = Base64Extensions.UrlSafeBase64Decode(encoded);

        // Assert
        plainText.ToArray().Should().BeEquivalentTo(Encoding.UTF8.GetBytes("14<+_~`/7'88"));
    }

    [Fact]
    public void UrlSafeBase64_UnderscoreSign_Recognise()
    {
        // Arrange
        var encoded = "WlNXRURYQ0ZSVEdWQkhZVUpOTUtJT0w8PjpQeyI_fH0rXykoKiZeJSTCo0Ah";

        // Act
        var plainText = Base64Extensions.UrlSafeBase64Decode(encoded);

        // Assert
        plainText.ToArray().Should().BeEquivalentTo(Encoding.UTF8.GetBytes("ZSWEDXCFRTGVBHYUJNMKIOL<>:P{\"?|}+_)(*&^%$£@!"));
    }

    [Fact]
    public void UrlSafeBase64_NoPadding_Recognise()
    {
        // Arrange
        var encoded = "MTIzNDU2Nzg5MA";

        // Act
        var plainText = Base64Extensions.UrlSafeBase64Decode(encoded);

        // Assert
        plainText.ToArray().Should().BeEquivalentTo(Encoding.UTF8.GetBytes("1234567890"));
    }
}