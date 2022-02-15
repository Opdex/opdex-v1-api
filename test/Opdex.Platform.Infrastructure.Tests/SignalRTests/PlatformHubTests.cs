using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;
using Opdex.Platform.Infrastructure.Clients.SignalR;
using SSAS.NET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.SignalRTests;

public class PlatformHubTests
{
    private readonly FakeTwoWayEncryptionProvider _twoWayEncryptionProvider;
    private readonly Mock<HubCallerContext> _hubCallerContextMock;
    private readonly PlatformHub _hub;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IPlatformClient> _callerClientMock;

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
            },
            Opdex = new AuthProvider
            {
                SigningKey = "xVaBi2bR5A7HMeiTSEeYBxV4Z9WEmpyR"
            }
        };
        _hubCallerContextMock = new Mock<HubCallerContext>();
        _callerClientMock = new Mock<IPlatformClient>();
        var hubCallerClientsMock = new Mock<IHubCallerClients<IPlatformClient>>();
        hubCallerClientsMock.Setup(callTo => callTo.Caller).Returns(_callerClientMock.Object);
        _mediatorMock = new Mock<IMediator>();
        _hub = new PlatformHub(_mediatorMock.Object, _twoWayEncryptionProvider, authConfiguration, opdexConfiguration)
        {
            Context = _hubCallerContextMock.Object,
            Clients = hubCallerClientsMock.Object
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

    [Fact]
    public async Task Reconnect_InvalidStratisId_DoNotAuthenticate()
    {
        // Arrange
        var previousConnectionId = "QU5FWENSWVBURURDT05ORUNUSU9OSUQ";
        var stratisId = "INVALID STRATIS ID";

        // Act
        var succeeded = await _hub.Reconnect(previousConnectionId, stratisId);

        // Assert
        succeeded.Should().Be(false);
        _callerClientMock.Verify(callTo => callTo.OnAuthenticated(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Reconnect_ValidStratisIdButExpired_DoNotAuthenticate()
    {
        // Arrange
        var unixTime10MinsAgo = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds();
        var previousConnectionId = "QU5FWENSWVBURURDT05ORUNUSU9OSUQ";
        var stratisId = $"sid:v1-dev-api.opdex.com/v1/auth/callback?uid=MtLXa7ZbmtGjKeCpZC-Y1cjNLDsVz4tDfBqahJssXOvsmUVSnYa5nclYnSZxhwcN1gjxrp4ydqoo3KRSKMdBaw&exp={unixTime10MinsAgo}";

        // Act
        var succeeded = await _hub.Reconnect(previousConnectionId, stratisId);

        // Assert
        succeeded.Should().Be(false);
        _callerClientMock.Verify(callTo => callTo.OnAuthenticated(It.IsAny<string>()), Times.Never);
    }

    // [Fact]
    // public async Task Reconnect_DifferentUid_DoNotAuthenticate()
    // {
    //     // Arrange
    //     var connectionId = "DIFFERENT CONNECTION ID";
    //     var unixTime10MinsFromNow = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();
    //     var previousConnectionId = "QU5FWENSWVBURURDT05ORUNUSU9OSUQ";
    //     var stratisId = $"sid:v1-dev-api.opdex.com/v1/auth/callback?uid=JztkuBy8zCCHSoPBmQ1D9YEUnNGYmRGE8j6EshsLRiSIF2aYLQiemjKsfHtqBFEJhxLjwtGRrzS3CZk6MDxa0A&exp={unixTime10MinsFromNow}";
    //
    //     // Act
    //     var succeeded = await _hub.Reconnect(previousConnectionId, stratisId);
    //
    //     // Assert
    //     succeeded.Should().Be(false);
    //     _callerClientMock.Verify(callTo => callTo.OnAuthenticated(It.IsAny<string>()), Times.Never);
    // }

    // [Fact]
    // public async Task Reconnect_NoAuthSuccessRecord_DoNotAuthenticate()
    // {
    //     // Arrange
    //     var unixTime10MinsFromNow = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();
    //     var previousConnectionId = "QU5FWENSWVBURURDT05ORUNUSU9OSUQ";
    //     var connectionId = "GO73rdOHET7W1FAuWp96Tw205af2011";
    //     var uid = "JztkuBy8zCCHSoPBmQ1D9YEUnNGYmRGE8j6EshsLRiSIF2aYLQiemjKsfHtqBFEJhxLjwtGRrzS3CZk6MDxa0A";
    //     var stratisId = $"sid:v1-dev-api.opdex.com/v1/auth/callback?uid={uid}&exp={unixTime10MinsFromNow}";
    //
    //     _twoWayEncryptionProvider.WhenEncryptCalled(() => new byte[]
    //     {
    //         39,  59, 100, 184, 28, 188, 204, 32, 135, 74, 131, 193, 153, 13, 67, 245,
    //         129, 20, 156, 209, 152, 153, 17, 132, 242, 62, 132, 178, 27, 11, 70, 36,
    //         136, 23, 102, 152, 45, 8, 158, 154, 50, 172, 124, 123, 106, 4, 81, 9,
    //         135, 18, 227, 194, 209, 145, 175, 52, 183, 9, 153, 58, 48, 60, 90, 208
    //     });
    //
    //     _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<SelectAuthSuccessByConnectionIdQuery>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync((AuthSuccess)null);
    //
    //     // Act
    //     var succeeded = await _hub.Reconnect(previousConnectionId, stratisId);
    //
    //     // Assert
    //     succeeded.Should().Be(false);
    //     _callerClientMock.Verify(callTo => callTo.OnAuthenticated(It.IsAny<string>()), Times.Never);
    // }
    //
    // [Fact]
    // public async Task Reconnect_ExpiredAuthSuccessRecord_DoNotAuthenticate()
    // {
    //     // Arrange
    //     var unixTime10MinsFromNow = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();
    //     var previousConnectionId = "QU5FWENSWVBURURDT05ORUNUSU9OSUQ";
    //     var connectionId = "GO73rdOHET7W1FAuWp96Tw205af2011";
    //     var uid = "JztkuBy8zCCHSoPBmQ1D9YEUnNGYmRGE8j6EshsLRiSIF2aYLQiemjKsfHtqBFEJhxLjwtGRrzS3CZk6MDxa0A";
    //     var stratisId = $"sid:v1-dev-api.opdex.com/v1/auth/callback?uid={uid}&exp={unixTime10MinsFromNow}";
    //
    //     _twoWayEncryptionProvider.WhenEncryptCalled(() => new byte[]
    //     {
    //         39,  59, 100, 184, 28, 188, 204, 32, 135, 74, 131, 193, 153, 13, 67, 245,
    //         129, 20, 156, 209, 152, 153, 17, 132, 242, 62, 132, 178, 27, 11, 70, 36,
    //         136, 23, 102, 152, 45, 8, 158, 154, 50, 172, 124, 123, 106, 4, 81, 9,
    //         135, 18, 227, 194, 209, 145, 175, 52, 183, 9, 153, 58, 48, 60, 90, 208
    //     });
    //
    //     _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<SelectAuthSuccessByConnectionIdQuery>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(new AuthSuccess(connectionId, new Address("PAe1RRxnRVZtbS83XQ4soyjwJUDSjaJAKZ"), DateTime.UtcNow.AddMinutes(-5)));
    //
    //     // Act
    //     var succeeded = await _hub.Reconnect(previousConnectionId, stratisId);
    //
    //     // Assert
    //     succeeded.Should().Be(false);
    //     _callerClientMock.Verify(callTo => callTo.OnAuthenticated(It.IsAny<string>()), Times.Never);
    // }
    //
    // [Fact]
    // public async Task Reconnect_Valid_Authenticate()
    // {
    //     // Arrange
    //     var unixTime10MinsFromNow = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();
    //     var previousConnectionId = "QU5FWENSWVBURURDT05ORUNUSU9OSUQ";
    //     var connectionId = "GO73rdOHET7W1FAuWp96Tw205af2011";
    //     var uid = "JztkuBy8zCCHSoPBmQ1D9YEUnNGYmRGE8j6EshsLRiSIF2aYLQiemjKsfHtqBFEJhxLjwtGRrzS3CZk6MDxa0A";
    //     var stratisId = $"sid:v1-dev-api.opdex.com/v1/auth/callback?uid={uid}&exp={unixTime10MinsFromNow}";
    //
    //     _twoWayEncryptionProvider.WhenEncryptCalled(() => new byte[]
    //     {
    //         39,  59, 100, 184, 28, 188, 204, 32, 135, 74, 131, 193, 153, 13, 67, 245,
    //         129, 20, 156, 209, 152, 153, 17, 132, 242, 62, 132, 178, 27, 11, 70, 36,
    //         136, 23, 102, 152, 45, 8, 158, 154, 50, 172, 124, 123, 106, 4, 81, 9,
    //         135, 18, 227, 194, 209, 145, 175, 52, 183, 9, 153, 58, 48, 60, 90, 208
    //     });
    //
    //     _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<SelectAuthSuccessByConnectionIdQuery>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(new AuthSuccess(connectionId, new Address("PAe1RRxnRVZtbS83XQ4soyjwJUDSjaJAKZ"), DateTime.UtcNow.AddMinutes(1)));
    //
    //     // Act
    //     var succeeded = await _hub.Reconnect(previousConnectionId, stratisId);
    //
    //     // Assert
    //     succeeded.Should().Be(true);
    //     _callerClientMock.Verify(callTo => callTo.OnAuthenticated(It.IsAny<string>()), Times.Once);
    // }
}

class FakeTwoWayEncryptionProvider : ITwoWayEncryptionProvider
{
    public Queue<byte[]> DecryptCalls = new();
    public Queue<string> EncryptCalls = new();

    private Func<string, byte[]> _encryptFunc = _ => Array.Empty<byte>();
    private Func<byte[], string> _decryptFunc = _ => "";

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

    public void WhenEncryptCalled(Func<byte[]> expression) => _encryptFunc = _ => expression.Invoke();
    public void WhenEncryptCalled(Func<string, byte[]> expression) => _encryptFunc = expression;

    public void WhenDecryptCalled(Func<string> expression) => _decryptFunc = _ => expression.Invoke();
    public void WhenDecryptCalled(Func<byte[], string> expression) => _decryptFunc = expression;
}
