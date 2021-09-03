using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using Opdex.Platform.Infrastructure.Data.Handlers.Governances;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Governances.Nominations
{
    public class SelectActiveMiningGovernanceNominationsQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectActiveMiningGovernanceNominationsQueryHandler _handler;

        public SelectActiveMiningGovernanceNominationsQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectActiveMiningGovernanceNominationsQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task  SelectActiveMiningGovernanceNominations_Success()
        {
            // Arrange
            var expected = new[]
            {
                new MiningGovernanceNominationEntity
                {
                    Id = 123454,
                    GovernanceId = 3,
                    LiquidityPoolId = 4,
                    MiningPoolId = 5,
                    IsNominated = true,
                    Weight = "10000000",
                    CreatedBlock = 1,
                    ModifiedBlock = 2
                }
            }.AsEnumerable();

            var command = new SelectActiveMiningGovernanceNominationsQuery();

            _dbContext.Setup(db => db.ExecuteQueryAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
                .ReturnsAsync(() => expected);

            // Act
            var results = await _handler.Handle(command, CancellationToken.None);

            // Assert
            results.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task  SelectActiveMiningGovernanceNominations_ReturnsEmpty()
        {
            // Arrange
            var command = new SelectActiveMiningGovernanceNominationsQuery();


            _dbContext.Setup(db => db.ExecuteQueryAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
                .ReturnsAsync(Enumerable.Empty<MiningGovernanceNominationEntity>);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
