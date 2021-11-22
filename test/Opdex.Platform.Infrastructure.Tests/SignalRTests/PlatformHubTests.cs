using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Clients.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.SignalRTests
{
    public class PlatformHubTests
    {
        private readonly FakeTwoWayEncryptionProvider _twoWayEncryptionProvider;
        private readonly Mock<HubCallerContext> _hubCallerContextMock;
        private readonly PlatformHub _hub;

        public PlatformHubTests()
        {
            _twoWayEncryptionProvider = new FakeTwoWayEncryptionProvider();
            _hubCallerContextMock = new Mock<HubCallerContext>();
            _hub = new PlatformHub(_twoWayEncryptionProvider)
            {
                Context = _hubCallerContextMock.Object
            };
        }

        [Fact]
        public void GetEncryptedConnectionId_TwoWayEncryptionProvider_Encrypt()
        {
            // Arrange
            var connectionId = "MY_C8NN3CTI8N_ID";
            _hubCallerContextMock.Setup(callTo => callTo.ConnectionId).Returns(connectionId);

            // Act
            var encoded = _hub.GetEncryptedConnectionId();

            // Assert
            _twoWayEncryptionProvider.EncryptCalls.Count.Should().Be(1);
            _twoWayEncryptionProvider.EncryptCalls.Dequeue().Should().Be(connectionId);
        }

        [Fact]
        public void GetEncryptedConnectionId_ConnectionId_UrlSafeBase64Encoded()
        {
            // Arrange
            _hubCallerContextMock.Setup(callTo => callTo.ConnectionId).Returns("8rn4UxxPl2m4jd8DDa9fir920");

            var expected = Encoding.UTF8.GetBytes("3NCRYPT3DCONNECTIONID");
            _twoWayEncryptionProvider.WhenEncryptCalled(() => expected);

            // Act
            var encrypted = _hub.GetEncryptedConnectionId();

            // Assert
            encrypted.Should().Be(Base64Extensions.UrlSafeBase64Encode(expected));
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
}