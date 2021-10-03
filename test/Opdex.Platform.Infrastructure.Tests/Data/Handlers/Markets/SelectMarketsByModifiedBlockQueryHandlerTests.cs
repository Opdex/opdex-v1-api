using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets
{
    public class SelectMarketsByModifiedBlockQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMarketsByModifiedBlockQueryHandler _handler;

        public SelectMarketsByModifiedBlockQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMarketsByModifiedBlockQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public void SelectMarketsByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new SelectMarketsByModifiedBlockQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task SelectMarketsByModifiedBlockQuery_ExecutesQuery()
        {
            // Arrange
            const ulong modifiedBlock = 10;
            var command = new SelectMarketsByModifiedBlockQuery(modifiedBlock);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MarketEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("ModifiedBlock = @ModifiedBlock") &&
                                                            q.Sql.Contains("FROM market"))), Times.Once);
        }

        [Fact]
        public async Task SelectMarketsByModifiedBlockQuery_Returns()
        {
            // Arrange
            const ulong modifiedBlock = 10;

            var entity = new MarketEntity
            {
                Id = 123454,
                Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
                DeployerId = 2,
                StakingTokenId = 3,
                Owner = "PHVEPMKehXfJ6u1yzNPDw7uGZPZpB4iW4L",
                AuthPoolCreators = true,
                AuthProviders = true,
                AuthTraders = false,
                TransactionFee = 3,
                MarketFeeEnabled = true,
                CreatedBlock = 10,
                ModifiedBlock = 11
            };

            var entities = new List<MarketEntity>
            {
                entity
            };

            var expectedResponse = new List<Market>
            {
                new Market(entity.Id, entity.Address, entity.DeployerId, entity.StakingTokenId, entity.PendingOwner, entity.Owner, entity.AuthPoolCreators,
                           entity.AuthProviders, entity.AuthTraders, entity.TransactionFee, entity.MarketFeeEnabled, entity.CreatedBlock, entity.ModifiedBlock)
            };

            var command = new SelectMarketsByModifiedBlockQuery(modifiedBlock);

            _dbContext.Setup(callTo => callTo.ExecuteQueryAsync<MarketEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync(entities);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
