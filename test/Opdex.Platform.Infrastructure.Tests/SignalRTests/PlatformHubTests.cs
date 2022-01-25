using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Clients.SignalR;
using SSAS.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.SignalRTests;

public class PlatformHubTests
{
    private readonly FakeTwoWayEncryptionProvider _twoWayEncryptionProvider;
    private readonly Mock<HubCallerContext> _hubCallerContextMock;
    private readonly PlatformHub _hub;

    private const string ApiUrlWithoutProtocol = "api.opdex.com";
    private const string ApiUrl = $"https://{ApiUrlWithoutProtocol}";
    private const string CallbackPath = "auth";

    public PlatformHubTests()
    {
        _twoWayEncryptionProvider = new FakeTwoWayEncryptionProvider();
        var opdexConfiguration = new OpdexConfiguration
        {
            ApiUrl = ApiUrl
        };
        var authConfiguration = new AuthConfiguration
        {
            StratisSignatureAuth = new StratisSignatureAuthConfiguration
            {
                CallbackPath = CallbackPath
            }
        };
        _hubCallerContextMock = new Mock<HubCallerContext>();
        _hub = new PlatformHub(_twoWayEncryptionProvider, authConfiguration, opdexConfiguration)
        {
            Context = _hubCallerContextMock.Object
        };
    }

    [Fact]
    public void GetStratisId_TwoWayEncryptionProvider_Encrypt()
    {
        // Arrange
        var connectionId = "MY_C8NN3CTI8N_ID";
        _hubCallerContextMock.Setup(callTo => callTo.ConnectionId).Returns(connectionId);

        // Act
        var encoded = _hub.GetStratisId();

        // Assert
        _twoWayEncryptionProvider.EncryptCalls.Count.Should().Be(1);
        var decrypted = _twoWayEncryptionProvider.EncryptCalls.Dequeue();

        var decryptedConnectionId = decrypted.Substring(0, decrypted.Length - 10);
        var decryptedExpiration = long.Parse(decrypted.Substring(decrypted.Length - 10));
        decryptedConnectionId.Should().Be(connectionId);
        decryptedExpiration.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetStratisId_StratisId_WellFormatted()
    {
        // Arrange
        _hubCallerContextMock.Setup(callTo => callTo.ConnectionId).Returns("8rn4UxxPl2m4jd8DDa9fir920");

        var expected = Encoding.UTF8.GetBytes("3NCRYPT3DCONNECTIONID");
        _twoWayEncryptionProvider.WhenEncryptCalled(() => expected);

        // Act
        var encrypted = _hub.GetStratisId();

        // Assert
        StratisId.TryParse(encrypted, out _).Should().Be(true);
    }

    [Fact]
    public void GetStratisId_Callback_FromConfig()
    {
        // Arrange
        _hubCallerContextMock.Setup(callTo => callTo.ConnectionId).Returns("8rn4UxxPl2m4jd8DDa9fir920");

        var expected = Encoding.UTF8.GetBytes("3NCRYPT3DCONNECTIONID");
        _twoWayEncryptionProvider.WhenEncryptCalled(() => expected);

        // Act
        var encrypted = _hub.GetStratisId();

        // Assert
        _ = StratisId.TryParse(encrypted, out var stratisId);
        stratisId.Callback.Should().StartWith(Path.Combine(ApiUrlWithoutProtocol, CallbackPath));
    }

    [Fact]
    public void GetStratisId_Uid_ConnectionIdUrlSafeBase64Encoded()
    {
        // Arrange
        _hubCallerContextMock.Setup(callTo => callTo.ConnectionId).Returns("8rn4UxxPl2m4jd8DDa9fir920");

        var expected = Encoding.UTF8.GetBytes("3NCRYPT3DCONNECTIONID");
        _twoWayEncryptionProvider.WhenEncryptCalled(() => expected);

        // Act
        var encrypted = _hub.GetStratisId();

        // Assert
        _ = StratisId.TryParse(encrypted, out var stratisId);
        stratisId.Uid.Should().Be(Base64Extensions.UrlSafeBase64Encode(expected));
    }
}

class FakeTwoWayEncryptionProvider : ITwoWayEncryptionProvider
{
    public Queue<byte[]> DecryptCalls = new();
    public Queue<string> EncryptCalls = new();

    private Func<string, byte[]> _encryptFunc = new(plainText => Array.Empty<byte>());
    private Func<byte[], string> _decryptFunc = new(encrypted => "");

    public string Decrypt(ReadOnlySpan<byte> cipherText)
    {
        DecryptCalls.Enqueue(cipherText.ToArray());
        return _decryptFunc.Invoke(cipherText.ToArray());
    }

    public ReadOnlySpan<byte> Encrypt(string plainText)
    {
        EncryptCalls.Enqueue(plainText);
        return _encryptFunc.Invoke(plainText).AsSpan();
    }

    public void WhenEncryptCalled(Func<byte[]> expression) => _encryptFunc = new Func<string, byte[]>(plainText => expression.Invoke());
    public void WhenEncryptCalled(Func<string, byte[]> expression) => _encryptFunc = expression;

    public void WhenDecryptCalled(Func<string> expression) => _decryptFunc = new Func<byte[], string>(cipherText => expression.Invoke());
    public void WhenDecryptCalled(Func<byte[], string> expression) => _decryptFunc = expression;
}
