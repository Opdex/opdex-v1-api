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
    public class RetrieveMarketPermissionsByUserQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveMarketPermissionsByUserQueryHandler _handler;

        public RetrieveMarketPermissionsByUserQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveMarketPermissionsByUserQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_MediatorSelectMarketPermissionsByUserQuery_Send()
        {
            // Arrange
            var marketId = 5L;
            var user = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new RetrieveMarketPermissionsByUserQuery(marketId, user), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(
                It.Is<SelectMarketPermissionsByUserQuery>(query => query.MarketId == marketId && query.User == user),
                cancellationToken
            ), Times.Once);
        }

        [Fact]
        public async Task Handle_MediatorSelectMarketPermissionsByUserQuery_Return()
        {
            // Arrange
            var permissions = new Permissions[] { Permissions.Provide, Permissions.Trade };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<SelectMarketPermissionsByUserQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(permissions);

            // Act
            var response = await _handler.Handle(new RetrieveMarketPermissionsByUserQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"), default);

            // Assert
            response.Should().BeEquivalentTo(permissions);
        }
    }
}
