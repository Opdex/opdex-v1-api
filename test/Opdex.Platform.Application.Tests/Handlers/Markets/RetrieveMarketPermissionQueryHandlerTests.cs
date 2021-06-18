using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Handlers.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Markets
{
    public class RetrieveMarketPermissionQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveMarketPermissionQueryHandler _handler;

        public RetrieveMarketPermissionQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveMarketPermissionQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_MediatorSelectMarketPermissionQuery_Send()
        {
            // Arrange
            var marketId = 10L;
            var address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var permission = Permissions.Provide;
            var findOrThrow = false;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new RetrieveMarketPermissionQuery(marketId, address, permission, findOrThrow), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<SelectMarketPermissionQuery>(query => query.MarketId == marketId
                                                         && query.Address == address
                                                         && query.Permission == permission
                                                         && query.FindOrThrow == findOrThrow),
                cancellationToken
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_MediatorSelectMarketPermissionQuery_Return()
        {
            // Arrange
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        Permissions.Trade,
                                                        true,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        500);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<SelectMarketPermissionQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(marketPermission);

            // Act
            var response = await _handler.Handle(new RetrieveMarketPermissionQuery(10,
                                                                                   "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                                                   Permissions.Provide,
                                                                                   false), default);

            // Assert
            response.Should().Be(marketPermission);
        }
    }
}