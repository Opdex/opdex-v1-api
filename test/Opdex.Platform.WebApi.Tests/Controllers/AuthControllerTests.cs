using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Auth;
using Opdex.Platform.WebApi.Auth;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Requests.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Opdex.Platform.WebApi.Auth.AuthConfiguration;

namespace Opdex.Platform.WebApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var configuration = new AuthConfiguration
            {
                Opdex = new AuthProvider
                {
                    SigningKey = "SECRET_SIGNING_KEY"
                },
                StratisOpenAuthProtocol = new StratisOpenAuthConfiguration
                {
                    CallbackBase = "api.opdex.com",
                }
            };

            _mediatorMock = new Mock<IMediator>();

            _controller = new AuthController(configuration, _mediatorMock.Object);
        }

        [Fact]
        public async Task StratisOpenAuthCallback_Expired_ThrowInvalidDataException()
        {
            // Arrange
            var query = new StratisOpenAuthCallbackQuery
            {
                Uid = Guid.NewGuid().ToString(),
                Exp = 1635200000 // 25 Oct 2021
            };
            var body = new StratisOpenAuthCallbackBody
            {
                PublicKey = new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
            };

            // Act
            Task Act() => _controller.StratisOpenAuthCallback(query, body, CancellationToken.None);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
            exception.PropertyName.Should().Be("exp");
        }

        [Fact]
        public async Task StratisOpenAuthCallback_CallCirrusVerifyMessageQuery_Send()
        {
            // Arrange
            var query = new StratisOpenAuthCallbackQuery
            {
                Uid = Guid.NewGuid().ToString(),
                Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds()
            };
            var body = new StratisOpenAuthCallbackBody
            {
                PublicKey = new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
            };

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _controller.StratisOpenAuthCallback(query, body, cancellationToken);
            }
            catch(Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusVerifyMessageQuery>(call => call.Message == $"api.opdex.com/auth?uid={query.Uid}&exp={query.Exp}"
                                                                                                && call.Signer == body.PublicKey
                                                                                                && call.Signature == body.Signature), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task StratisOpenAuthCallback_InvalidSignature_ThrowInvalidDataException()
        {
            // Arrange
            var query = new StratisOpenAuthCallbackQuery
            {
                Uid = Guid.NewGuid().ToString(),
                Exp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds()
            };
            var body = new StratisOpenAuthCallbackBody
            {
                PublicKey = new Address("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"),
                Signature = "H9xjfnvqucCmi3sfEKUes0qL4mD9PrZ/al78+Ka440t6WH5Qh0AIgl5YlxPa2cyuXdwwDa2OYUWR/0ocL6jRZLc="
            };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusVerifyMessageQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            Task Act() => _controller.StratisOpenAuthCallback(query, body, CancellationToken.None);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
            exception.PropertyName.Should().Be("signature");
        }
    }
}