using MediatR;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using Opdex.Platform.Infrastructure.Clients.SignalR;
using Opdex.Platform.Infrastructure.Clients.SignalR.Handlers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.SignalRTests.Handlers;

public class NotifyUserOfBroadcastTransactionCommandHandlerTests
{
    private readonly Mock<IPlatformClient> _clientMock;
    private readonly Mock<IHubClients<IPlatformClient>> _hubClientsMock;
    private readonly Mock<IHubContext<PlatformHub, IPlatformClient>> _hubContextMock;
    private readonly IRequestHandler<NotifyUserOfBroadcastTransactionCommand> _handler;

    public NotifyUserOfBroadcastTransactionCommandHandlerTests()
    {
        _clientMock = new Mock<IPlatformClient>();
        _hubClientsMock = new Mock<IHubClients<IPlatformClient>>();
        _hubClientsMock.Setup(callTo => callTo.User(It.IsAny<string>())).Returns(_clientMock.Object);
        _hubContextMock = new Mock<IHubContext<PlatformHub, IPlatformClient>>();
        _hubContextMock.SetupGet(callTo => callTo.Clients).Returns(_hubClientsMock.Object);
        _handler = new NotifyUserOfBroadcastTransactionCommandHandler(_hubContextMock.Object);
    }

    [Fact]
    public async Task Handle_NotifyUser_WithTransactionHash()
    {
        // Arrange
        var request = new NotifyUserOfBroadcastTransactionCommand("PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", new Sha256(324394839));

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _hubContextMock.Verify(callTo => callTo.Clients.User(request.User.ToString()).OnTransactionBroadcast(request.TxHash.ToString()), Times.Once);
    }
}