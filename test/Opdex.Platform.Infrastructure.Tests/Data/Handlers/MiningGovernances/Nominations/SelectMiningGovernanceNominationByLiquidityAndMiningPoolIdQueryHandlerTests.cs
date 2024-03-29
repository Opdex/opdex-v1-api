using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances.Nominations;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernances.Nominations;

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
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(5, 5, 5, false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectMiningGovernanceNominationByLiquidityAndMiningPoolId_Success()
    {
        // Arrange
        const ulong miningGovernanceId = 3;
        const ulong liquidityPoolId = 4;
        const ulong miningPoolId = 5;

        var expectedEntity = new MiningGovernanceNominationEntity
        {
            Id = 123454,
            MiningGovernanceId = miningGovernanceId,
            LiquidityPoolId = liquidityPoolId,
            MiningPoolId = miningPoolId,
            IsNominated = true,
            Weight = 10000000,
            CreatedBlock = 1,
            ModifiedBlock = 2,
        };

        var command = new SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(miningGovernanceId, liquidityPoolId, miningPoolId);

        _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().Be(expectedEntity.Id);
        result.MiningGovernanceId.Should().Be(expectedEntity.MiningGovernanceId);
        result.LiquidityPoolId.Should().Be(expectedEntity.LiquidityPoolId);
        result.MiningPoolId.Should().Be(expectedEntity.MiningPoolId);
        result.IsNominated.Should().Be(expectedEntity.IsNominated);
        result.Weight.Should().Be(expectedEntity.Weight);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public async Task SelectMiningGovernanceNominationByLiquidityAndMiningPoolId_Throws_NotFoundException()
    {
        // Arrange
        var command = new SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(5, 10, 15);

        _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<MiningGovernanceNominationEntity>(null));

        // Act
        // Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Mining governance nomination not found.");
    }

    [Fact]
    public async Task SelectMiningGovernanceNominationByLiquidityAndMiningPoolId_ReturnsNull()
    {
        // Arrange
        const bool findOrThrow = false;

        var command = new SelectMiningGovernanceNominationByLiquidityAndMiningPoolIdQuery(5, 10, 15, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceNominationEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<MiningGovernanceNominationEntity>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
