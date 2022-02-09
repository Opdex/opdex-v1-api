using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools;

public class SelectLiquidityPoolBySrcTokenIdAndMarketIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler _handler;

    public SelectLiquidityPoolBySrcTokenIdAndMarketIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectLiquidityPoolBySrcTokenIdAndMarketIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(5, 5, false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectLiquidityPoolBySrcTokenIdAndMarketId_Success()
    {
        const ulong srcTokenId = 99;
        const ulong marketId = 100;

        var expectedEntity = new LiquidityPoolEntity
        {
            Id = 123454,
            SrcTokenId = 1235,
            LpTokenId = 8765,
            MarketId = 1,
            Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
            CreatedBlock = 1,
            ModifiedBlock = 1
        };

        var command = new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(srcTokenId, marketId);

        _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult(expectedEntity));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Id.Should().Be(expectedEntity.Id);
        result.SrcTokenId.Should().Be(expectedEntity.SrcTokenId);
        result.LpTokenId.Should().Be(expectedEntity.LpTokenId);
        result.MarketId.Should().Be(expectedEntity.MarketId);
        result.Address.Should().Be(expectedEntity.Address);
        result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
        result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
    }

    [Fact]
    public void SelectLiquidityPoolBySrcTokenIdAndMarketId_Throws_NotFoundException()
    {
        const ulong srcTokenId = 99;
        const ulong marketId = 100;

        var command = new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(srcTokenId, marketId);

        _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<LiquidityPoolEntity>(null));

        _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(LiquidityPool)} not found.");
    }

    [Fact]
    public async Task SelectLiquidityPoolBySrcTokenIdAndMarketId_ReturnsNull()
    {
        const ulong srcTokenId = 99;
        const ulong marketId = 100;
        const bool findOrThrow = false;

        var command = new SelectLiquidityPoolBySrcTokenIdAndMarketIdQuery(srcTokenId, marketId, findOrThrow);

        _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
            .Returns(() => Task.FromResult<LiquidityPoolEntity>(null));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeNull();
    }
}
