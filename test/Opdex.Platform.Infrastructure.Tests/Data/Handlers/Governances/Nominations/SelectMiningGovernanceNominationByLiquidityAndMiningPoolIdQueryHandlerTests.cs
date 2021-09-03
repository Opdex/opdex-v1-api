using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using Opdex.Platform.Infrastructure.Data.Handlers.Governances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Governances.Nominations
{
    public class SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler _handler;

        public SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectMiningGovernanceNominationByLiquidityAndMiningPoolId_Success()
        {
            // Arrange
            const long liquidityPoolId = 4;
            const long miningPoolId = 5;

            var expectedEntity = new MiningGovernanceNominationEntity
            {
                Id = 123454,
                GovernanceId = 3,
                LiquidityPoolId = liquidityPoolId,
                MiningPoolId = miningPoolId,
                IsNominated = true,
                Weight = "10000000",
                CreatedBlock = 1,
                ModifiedBlock = 2,
            };

            var command = new SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(liquidityPoolId, miningPoolId);

            _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedEntity));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Id.Should().Be(expectedEntity.Id);
            result.GovernanceId.Should().Be(expectedEntity.GovernanceId);
            result.LiquidityPoolId.Should().Be(expectedEntity.LiquidityPoolId);
            result.MiningPoolId.Should().Be(expectedEntity.MiningPoolId);
            result.IsNominated.Should().Be(expectedEntity.IsNominated);
            result.Weight.Should().Be(expectedEntity.Weight);
            result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
            result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
        }

        [Fact]
        public void SelectMiningGovernanceNominationByLiquidityAndMiningPoolId_Throws_NotFoundException()
        {
            // Arrange
            var command = new SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(10, 15);

            _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MiningGovernanceNominationEntity>(null));

            // Act
            // Assert
            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(MiningGovernanceNomination)} not found.");
        }

        [Fact]
        public async Task SelectMiningGovernanceNominationByLiquidityAndMiningPoolId_ReturnsNull()
        {
            // Arrange
            const bool findOrThrow = false;

            var command = new SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(10, 15, findOrThrow);

            _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MiningGovernanceNominationEntity>(null));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
