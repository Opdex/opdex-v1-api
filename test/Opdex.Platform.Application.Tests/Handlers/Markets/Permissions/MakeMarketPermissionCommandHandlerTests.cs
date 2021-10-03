using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Handlers.Markets.Permissions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Markets.Permissions
{
    public class MakeMarketPermissionCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly MakeMarketPermissionCommandHandler _handler;

        public MakeMarketPermissionCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new MakeMarketPermissionCommandHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_MediatorPersistMarketPermissionCommand_Send()
        {
            // Arrange
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        MarketPermissionType.Trade,
                                                        true,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        500);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new MakeMarketPermissionCommand(marketPermission), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<PersistMarketPermissionCommand>(command => command.MarketPermission == marketPermission),
                cancellationToken
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_MediatorPersistMarketPermissionCommand_Return()
        {
            // Arrange
            var id = 5ul;
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        MarketPermissionType.Trade,
                                                        true,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        500);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistMarketPermissionCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(id);

            // Act
            var response = await _handler.Handle(new MakeMarketPermissionCommand(marketPermission), default);

            // Assert
            response.Should().Be(id);
        }
    }
}
