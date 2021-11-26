using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernances
{
    public class SelectMiningGovernancesByModifiedBlockQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMiningGovernancesByModifiedBlockQueryHandler _handler;

        public SelectMiningGovernancesByModifiedBlockQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMiningGovernancesByModifiedBlockQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public void SelectMiningGovernancesByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new SelectMiningGovernancesByModifiedBlockQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task SelectMiningGovernancesByModifiedBlockQuery_ExecutesQuery()
        {
            // Arrange
            const ulong modifiedBlock = 10;
            var command = new SelectMiningGovernancesByModifiedBlockQuery(modifiedBlock);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MiningGovernanceEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("ModifiedBlock = @ModifiedBlock") &&
                                                            q.Sql.Contains("FROM governance"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningGovernancesByModifiedBlockQuery_Returns()
        {
            // Arrange
            const ulong modifiedBlock = 10;

            var entities = new List<MiningGovernanceEntity>
            {
                new MiningGovernanceEntity
                {
                    Id = 123454,
                    Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
                    TokenId = 3,
                    NominationPeriodEnd = 999,
                    MiningDuration = 1444,
                    MiningPoolsFunded = 10,
                    MiningPoolReward = 876543456789,
                    CreatedBlock = 1,
                    ModifiedBlock = 4
                }
            };

            var expectedResponse = new List<MiningGovernance>
            {
                new MiningGovernance(123454, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 3, 999, 1444, 10, 876543456789, 1, 4)
            };

            var command = new SelectMiningGovernancesByModifiedBlockQuery(modifiedBlock);

            _dbContext.Setup(callTo => callTo.ExecuteQueryAsync<MiningGovernanceEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync(entities);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
