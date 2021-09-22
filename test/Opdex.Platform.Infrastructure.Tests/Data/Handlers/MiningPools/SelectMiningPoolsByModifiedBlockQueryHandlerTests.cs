using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningPools;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningPools
{
    public class SelectMiningPoolsByModifiedBlockQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMiningPoolsByModifiedBlockQueryHandler _handler;

        public SelectMiningPoolsByModifiedBlockQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMiningPoolsByModifiedBlockQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public void SelectMiningPoolsByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new SelectMiningPoolsByModifiedBlockQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task SelectMiningPoolsByModifiedBlockQuery_ExecutesQuery()
        {
            // Arrange
            const ulong modifiedBlock = 10;
            var command = new SelectMiningPoolsByModifiedBlockQuery(modifiedBlock);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("ModifiedBlock = @ModifiedBlock") &&
                                                            q.Sql.Contains("FROM pool_mining"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPoolsByModifiedBlockQuery_Returns()
        {
            // Arrange
            const ulong modifiedBlock = 10;

            var entities = new List<MiningPoolEntity>
            {
                new MiningPoolEntity
                {
                    Id = 123454,
                    Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
                    LiquidityPoolId = 2,
                    RewardPerBlock = 10,
                    RewardPerLpt = 1,
                    MiningPeriodEndBlock = 100,
                    CreatedBlock = 2,
                    ModifiedBlock = 4
                }
            };

            var expectedResponse = new List<MiningPool>
            {
                new MiningPool(123454, 2, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 1, 100, 2, 4)
            };

            var command = new SelectMiningPoolsByModifiedBlockQuery(modifiedBlock);

            _dbContext.Setup(callTo => callTo.ExecuteQueryAsync<MiningPoolEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync(entities);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
