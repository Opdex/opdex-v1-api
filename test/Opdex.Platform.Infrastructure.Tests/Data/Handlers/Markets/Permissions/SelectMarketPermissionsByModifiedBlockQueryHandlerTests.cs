using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Permissions;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Permissions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets.Permissions
{
    public class SelectMarketPermissionsByModifiedBlockQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMarketPermissionsByModifiedBlockQueryHandler _handler;

        public SelectMarketPermissionsByModifiedBlockQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMarketPermissionsByModifiedBlockQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public void SelectMarketPermissionsByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new SelectMarketPermissionsByModifiedBlockQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task SelectMarketPermissionsByModifiedBlockQuery_ExecutesQuery()
        {
            // Arrange
            const ulong modifiedBlock = 10;
            var command = new SelectMarketPermissionsByModifiedBlockQuery(modifiedBlock);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MarketPermissionEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("ModifiedBlock = @ModifiedBlock") &&
                                                            q.Sql.Contains("FROM market_permission"))), Times.Once);
        }

        [Fact]
        public async Task SelectMarketPermissionsByModifiedBlockQuery_Returns()
        {
            // Arrange
            const ulong modifiedBlock = 10;

            var entity = new MarketPermissionEntity
            {
                Id = 1,
                Blame = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
                IsAuthorized = true,
                MarketId = 2,
                Permission = 3,
                CreatedBlock = 4,
                ModifiedBlock = 5,
                User = "PMKehXfJ6u1yzNPDw7uGZPZpB4iW4LHVEP"
            };

            var entities = new List<MarketPermissionEntity> { entity };

            var expectedResponse = new List<MarketPermission>
            {
                new MarketPermission(entity.Id, entity.MarketId, entity.User, (MarketPermissionType)entity.Permission, entity.IsAuthorized,
                                     entity.Blame, entity.CreatedBlock, entity.ModifiedBlock )
            };

            var command = new SelectMarketPermissionsByModifiedBlockQuery(modifiedBlock);

            _dbContext.Setup(callTo => callTo.ExecuteQueryAsync<MarketPermissionEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync(entities);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
