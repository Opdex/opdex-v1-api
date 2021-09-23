using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Permissions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets.Permissions
{
    public class SelectMarketPermissionsByUserQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMarketPermissionsByUserQueryHandler _handler;

        public SelectMarketPermissionsByUserQueryHandlerTests()
        {
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMarketPermissionsByUserQueryHandler(_dbContext.Object);
        }

        [Fact]
        public async Task Handle_HappyPath_ExecuteQuery()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            var result = await _handler.Handle(
                new SelectMarketPermissionsByUserQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"),
                cancellationToken);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MarketPermissionType>(
                It.Is<DatabaseQuery>(query => query.Token == cancellationToken)), Times.Once);
        }

        [Fact]
        public async Task Handle_NoResult_ReturnEmpty()
        {
            // Arrange
            _dbContext.Setup(callTo => callTo.ExecuteQueryAsync<MarketPermissionType>(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(Enumerable.Empty<MarketPermissionType>());

            // Act
            var result = await _handler.Handle(
                new SelectMarketPermissionsByUserQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"),
                default);

            // Assert
            result.Should().BeEquivalentTo(Enumerable.Empty<MarketPermissionType>());
        }

        [Fact]
        public async Task Handle_FoundPermissions_ReturnUnmodified()
        {
            // Arrange
            var permissions = new MarketPermissionType[] { MarketPermissionType.Provide, MarketPermissionType.SetPermissions, MarketPermissionType.CreatePool };
            _dbContext.Setup(callTo => callTo.ExecuteQueryAsync<MarketPermissionType>(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(permissions);

            // Act
            var result = await _handler.Handle(
                new SelectMarketPermissionsByUserQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"),
                default);

            // Assert
            result.Should().BeEquivalentTo(permissions);
        }
    }
}
