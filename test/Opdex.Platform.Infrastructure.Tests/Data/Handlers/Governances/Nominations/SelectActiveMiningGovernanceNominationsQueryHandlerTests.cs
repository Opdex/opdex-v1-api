using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances.Nominations;
using Opdex.Platform.Infrastructure.Data.Handlers.Governances.Nominations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Governances.Nominations
{
    public class SelectActiveGovernanceNominationsByGovernanceIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectActiveGovernanceNominationsByGovernanceIdQueryHandler _handler;

        public SelectActiveGovernanceNominationsByGovernanceIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectActiveGovernanceNominationsByGovernanceIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectActiveMiningGovernanceNominations_Success()
        {
            // Arrange
            const ulong governanceId = 3;

            var expected = new[]
            {
                new MiningGovernanceNominationEntity
                {
                    Id = 123454,
                    GovernanceId = governanceId,
                    LiquidityPoolId = 4,
                    MiningPoolId = 5,
                    IsNominated = true,
                    Weight = 10000000,
                    CreatedBlock = 1,
                    ModifiedBlock = 2
                }
            }.AsEnumerable();

            var command = new SelectActiveGovernanceNominationsByGovernanceIdQuery(governanceId);

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
            const ulong governanceId = 3;
            var command = new SelectActiveGovernanceNominationsByGovernanceIdQuery(governanceId);

            _dbContext.Setup(db => db.ExecuteQueryAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
                .ReturnsAsync(Enumerable.Empty<MiningGovernanceNominationEntity>);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
