using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using Opdex.Platform.WebApi.Controllers;
using SSAS.NET;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly FakeTwoWayEncryptionProvider _fakeTwoWayEncryptionProvider;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        var opdexConfiguration = new OpdexConfiguration
        {
            ApiUrl = "https://api.opdex.com:44392"
        };

        var authConfiguration = new AuthConfiguration
        {
            Opdex = new AuthProvider
            {
                SigningKey = "SECRET_SIGNING_KEY"
            },
            StratisSignatureAuth = new StratisSignatureAuthConfiguration
            {
                CallbackPath = "auth",
            }
        };

        _mediatorMock = new Mock<IMediator>();
        _fakeTwoWayEncryptionProvider = new FakeTwoWayEncryptionProvider();

        _controller = new AuthController(opdexConfiguration, authConfiguration, Mock.Of<ILogger<AuthController>>(), _mediatorMock.Object, _fakeTwoWayEncryptionProvider);
    }

    [Fact]
    public async Task StratisSignatureAuthCallback_Expired_ThrowInvalidDataException()
    {
        // Arrange
        var query = new StratisSignatureAuthCallbackQuery
        {
            Uid = Guid.NewGuid().ToString(),
            Exp = 1635200000 // 25 Oct 2021
        };
        var body = new StratisSignatureAuthCallbackBody
        {
            PublicKey = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
            Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
        };

        // Act
        Task Act() => _controller.StratisSignatureAuthCallback(query, body, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
        exception.PropertyName.Should().Be("exp");
    }

    [Fact]
    public async Task StratisSignatureAuthCallback_CallCirrusVerifyMessageQuery_Send()
    {
        // Arrange
        var query = new StratisSignatureAuthCallbackQuery
        {
            Uid = Guid.NewGuid().ToString(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds()
        };
        var body = new StratisSignatureAuthCallbackBody
        {
            PublicKey = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
            Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
        };

        var cancellationToken = CancellationToken.None;
        using (var cts = new CancellationTokenSource()) { cancellationToken = cts.Token; }

        // Act
        try
        {
            await _controller.StratisSignatureAuthCallback(query, body, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusVerifyMessageQuery>(call => call.Message == $"api.opdex.com:44392/auth?uid={query.Uid}&exp={query.Exp}"
                                                                                               && call.Signer == body.PublicKey
                                                                                               && call.Signature == body.Signature), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StratisSignatureAuthCallback_InvalidSignature_ThrowInvalidDataException()
    {
        // Arrange
        var query = new StratisSignatureAuthCallbackQuery
        {
            Uid = Guid.NewGuid().ToString(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds()
        };
        var body = new StratisSignatureAuthCallbackBody
        {
            PublicKey = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
            Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusVerifyMessageQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        Task Act() => _controller.StratisSignatureAuthCallback(query, body, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
        exception.PropertyName.Should().Be("signature");
    }

    [Fact]
    public async Task StratisSignatureAuthCallback_Uid_Decrypt()
    {
        // Arrange
        var query = new StratisSignatureAuthCallbackQuery
        {
            Uid = Guid.NewGuid().ToString(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds()
        };
        var body = new StratisSignatureAuthCallbackBody
        {
            PublicKey = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
            Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusVerifyMessageQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _fakeTwoWayEncryptionProvider.WhenDecryptCalled(() => "CONNECTION_ID");

        // Act
        await _controller.StratisSignatureAuthCallback(query, body, CancellationToken.None);

        // Assert
        _fakeTwoWayEncryptionProvider.DecryptCalls.Count.Should().Be(1);
        _fakeTwoWayEncryptionProvider.DecryptCalls.Dequeue().Should().BeEquivalentTo(Base64Extensions.UrlSafeBase64Decode(query.Uid).ToArray());
    }

    [Fact]
    public async Task StratisSignatureAuthCallback_DecryptionError_ThrowInvalidDataException()
    {
        // Arrange
        var query = new StratisSignatureAuthCallbackQuery
        {
            Uid = Guid.NewGuid().ToString(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds()
        };
        var body = new StratisSignatureAuthCallbackBody
        {
            PublicKey = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
            Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusVerifyMessageQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _fakeTwoWayEncryptionProvider.WhenDecryptCalled(() => throw new CryptographicException("Invalid key."));

        // Act
        Task Act() => _controller.StratisSignatureAuthCallback(query, body, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
        exception.PropertyName.Should().Be("uid");
    }

    [Fact]
    public async Task StratisSignatureAuthCallback_NotifyUserOfSuccessfulAuthenticationCommand_Send()
    {
        // Arrange
        var query = new StratisSignatureAuthCallbackQuery
        {
            Uid = Guid.NewGuid().ToString(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds()
        };
        var body = new StratisSignatureAuthCallbackBody
        {
            PublicKey = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
            Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
        };

        var connectionId = "CONNECTION_ID";

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusVerifyMessageQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _fakeTwoWayEncryptionProvider.WhenDecryptCalled(() => connectionId);

        var cancellationToken = CancellationToken.None;
        using (var cts = new CancellationTokenSource()) { cancellationToken = cts.Token; }

        // Act
        await _controller.StratisSignatureAuthCallback(query, body, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
                                 It.Is<NotifyUserOfSuccessfulAuthenticationCommand>(command => command.ConnectionId == connectionId), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task StratisSignatureAuthCallback_OnSuccess_NoContent()
    {
        // Arrange
        var query = new StratisSignatureAuthCallbackQuery
        {
            Uid = Guid.NewGuid().ToString(),
            Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds()
        };
        var body = new StratisSignatureAuthCallbackBody
        {
            PublicKey = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
            Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
        };

        var connectionId = "CONNECTION_ID";

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusVerifyMessageQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _fakeTwoWayEncryptionProvider.WhenDecryptCalled(() => connectionId);

        // Act
        var response = await _controller.StratisSignatureAuthCallback(query, body, CancellationToken.None);

        // Assert
        response.Should().BeAssignableTo<NoContentResult>();
    }
}

class FakeTwoWayEncryptionProvider : ITwoWayEncryptionProvider
{
    public Queue<byte[]> DecryptCalls = new Queue<byte[]>();
    public Queue<string> EncryptCalls = new Queue<string>();

    private Func<string, byte[]> _encryptFunc = new Func<string, byte[]>(plainText => Array.Empty<byte>());
    private Func<byte[], string> _decryptFunc = new Func<byte[], string>(encrypted => "");

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