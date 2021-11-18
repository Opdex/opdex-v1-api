using MediatR;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using Opdex.Platform.Infrastructure.Clients.SignalR;
using Opdex.Platform.Infrastructure.Clients.SignalR.Handlers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.SignalRTests.Handlers
{
    public class NotifyUserOfSuccessfulAuthenticationCommandHandlerTests
    {
        private readonly Mock<IPlatformClient> _clientMock;
        private readonly Mock<IHubClients<IPlatformClient>> _hubClientsMock;
        private readonly Mock<IHubContext<PlatformHub, IPlatformClient>> _hubContextMock;
        private readonly IRequestHandler<NotifyUserOfSuccessfulAuthenticationCommand> _handler;

        public NotifyUserOfSuccessfulAuthenticationCommandHandlerTests()
        {
            _clientMock = new Mock<IPlatformClient>();
            _hubClientsMock = new Mock<IHubClients<IPlatformClient>>();
            _hubClientsMock.Setup(callTo => callTo.Client(It.IsAny<string>())).Returns(_clientMock.Object);
            _hubContextMock = new Mock<IHubContext<PlatformHub, IPlatformClient>>();
            _hubContextMock.SetupGet(callTo => callTo.Clients).Returns(_hubClientsMock.Object);
            _handler = new NotifyUserOfSuccessfulAuthenticationCommandHandler(_hubContextMock.Object);
        }

        [Fact]
        public async Task Handle_NotifyUser_WithTransactionHash()
        {
            // Arrange
            var request = new NotifyUserOfSuccessfulAuthenticationCommand(Guid.NewGuid().ToString(), "SECRET_BEARER_TOKEN");

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _hubContextMock.Verify(callTo => callTo.Clients.Client(request.ConnectionId.ToString()).OnAuthenticated(request.BearerToken.ToString()), Times.Once);
        }
    }
}
