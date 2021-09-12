using Microsoft.AspNetCore.SignalR;
using Moq;
using Opdex.Platform.Domain.Events;
using Opdex.Platform.Infrastructure.Clients.SignalR;
using Opdex.Platform.Infrastructure.Clients.SignalR.Handlers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.SignalRTests.Handlers
{
    public class NotifyClientOfBroadcastTransactionHandlerTests
    {
        private readonly Mock<IPlatformClient> _clientMock;
        private readonly Mock<IHubClients<IPlatformClient>> _hubClientsMock;
        private readonly Mock<IHubContext<PlatformHub, IPlatformClient>> _hubContextMock;
        private readonly NotifyClientOfBroadcastTransactionHandler _handler;

        public NotifyClientOfBroadcastTransactionHandlerTests()
        {
            _clientMock = new Mock<IPlatformClient>();
            _hubClientsMock = new Mock<IHubClients<IPlatformClient>>();
            _hubClientsMock.Setup(callTo => callTo.User(It.IsAny<string>())).Returns(_clientMock.Object);
            _hubContextMock = new Mock<IHubContext<PlatformHub, IPlatformClient>>();
            _hubContextMock.SetupGet(callTo => callTo.Clients).Returns(_hubClientsMock.Object);
            _handler = new NotifyClientOfBroadcastTransactionHandler(_hubContextMock.Object);
        }

        [Fact]
        public async Task Handle_NotifyUser_WithTransactionHash()
        {
            // Arrange
            var notification = new TransactionBroadcastNotification("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "txHash12345");

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _hubContextMock.Verify(callTo => callTo.Clients.User(notification.User.ToString()).OnTransactionBroadcast(notification.TxHash), Times.Once);
        }
    }
}
