using FluentAssertions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Encryption;
using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Opdex.Platform.Common.Tests.Encryption;

public class AesCbcProviderTests : IDisposable
{
    private readonly EncryptionConfiguration _encryptionConfiguration;
    private readonly AesCbcProvider _aesCbcProvider;

    public AesCbcProviderTests()
    {
        _encryptionConfiguration = new EncryptionConfiguration { Key = "0123456789ABCDEF0123456789ABCDEF" };
        _aesCbcProvider = new AesCbcProvider(_encryptionConfiguration);
    }

    [Theory]
    [InlineData("_HAS_LENGTH_15_")]
    [InlineData("_16_CHARS_LONG_")]
    [InlineData("_SEVENTEEN_CHARS_")]
    public void Encrypt_VariableLengthPlaintext_AesCbcResult(string plainText)
    {
        // Arrange
        // Act
        var encrypted = _aesCbcProvider.Encrypt(plainText);

        // Assert
        const int blockSize = 16;
        const int ivLength = 16;
        var connectionIdBytes = Encoding.UTF8.GetBytes(plainText).Length;
        var cipherLength = connectionIdBytes + (connectionIdBytes % blockSize == 0 ? 0 : blockSize - (connectionIdBytes % blockSize));
        encrypted.Length.Should().Be(cipherLength + ivLength);
    }

    [Fact]
    public void Encrypt_EncryptTwice_DifferentIV()
    {
        // Arrange
        var plainText = "PLAINTEXT";

        // Act
        var encryptedA = _aesCbcProvider.Encrypt(plainText);
        var encryptedB = _aesCbcProvider.Encrypt(plainText);

        // Assert
        (encryptedA != encryptedB).Should().Be(true);
    }

    [Fact]
    public void Decrypt_SameProviderRoundtrip_Success()
    {
        // Arrange
        var plainText = "PLAINTEXT";
        var encrypted = _aesCbcProvider.Encrypt(plainText);

        // Act
        var decrypted = _aesCbcProvider.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Decrypt_DifferentProviderSameKey_Success()
    {
        // Arrange
        var plainText = "VERY_SECRET_SECRET";
        var encrypted = _aesCbcProvider.Encrypt(plainText);

        // Act
        string decrypted = null;
        using (var aesCbcProviderTwo = new AesCbcProvider(_encryptionConfiguration))
        {
            decrypted = aesCbcProviderTwo.Decrypt(encrypted);
        }

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Decrypt_DifferentProviderDifferentKey_ThrowCryptographicException()
    {
        // Arrange
        var plainText = "VERY_SECRET_SECRET";
        var encrypted = _aesCbcProvider.Encrypt(plainText).ToArray();

        // Act
        void Act()
        {
            using var aesCbcProviderTwo = new AesCbcProvider(new EncryptionConfiguration { Key = "D1FFD1FFD1FFD1FFD1FFD1FFD1FFD1FF" });
            var decrypted = aesCbcProviderTwo.Decrypt(encrypted);
        }

        // Assert
        Assert.Throws<CryptographicException>(Act);
    }

    public void Dispose()
    {
        _aesCbcProvider.Dispose();
    }
}