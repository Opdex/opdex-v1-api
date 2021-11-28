using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances.Nominations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernances.Nominations
{
    public class SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandler _handler;

        public SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectActiveMiningGovernanceNominations_Success()
        {
            // Arrange
            const ulong miningGovernanceId = 3;

            var expected = new[]
            {
                new MiningGovernanceNominationEntity
                {
                    Id = 123454,
                    MiningGovernanceId = miningGovernanceId,
                    LiquidityPoolId = 4,
                    MiningPoolId = 5,
                    IsNominated = true,
                    Weight = 10000000,
                    CreatedBlock = 1,
                    ModifiedBlock = 2
                }
            }.AsEnumerable();

            var command = new SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQuery(miningGovernanceId);

            _dbContext.Setup(db => db.ExecuteQueryAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
                .ReturnsAsync(() => expected);

            // Act
            var results = await _handler.Handle(command, CancellationToken.None);

            // Assert
            results.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task SelectActiveMiningGovernanceNominations_ReturnsEmpty()
        {
            // Arrange
            const ulong miningGovernanceId = 3;
            var command = new SelectActiveMiningGovernanceNominationsByMiningGovernanceIdQuery(miningGovernanceId);

            _dbContext.Setup(db => db.ExecuteQueryAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
                .ReturnsAsync(Enumerable.Empty<MiningGovernanceNominationEntity>);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
