using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets
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
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<Permissions>(
                It.Is<DatabaseQuery>(query => query.Token == cancellationToken)), Times.Once);
        }

        [Fact]
        public async Task Handle_NoResult_ReturnEmpty()
        {
            // Arrange
            _dbContext.Setup(callTo => callTo.ExecuteQueryAsync<Permissions>(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(Enumerable.Empty<Permissions>());

            // Act
            var result = await _handler.Handle(
                new SelectMarketPermissionsByUserQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"),
                default);

            // Assert
            result.Should().BeEquivalentTo(Enumerable.Empty<Permissions>());
        }

        [Fact]
        public async Task Handle_FoundPermissions_ReturnUnmodified()
        {
            // Arrange
            var permissions = new Permissions[] { Permissions.Provide, Permissions.SetPermissions, Permissions.CreatePool };
            _dbContext.Setup(callTo => callTo.ExecuteQueryAsync<Permissions>(It.IsAny<DatabaseQuery>()))
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