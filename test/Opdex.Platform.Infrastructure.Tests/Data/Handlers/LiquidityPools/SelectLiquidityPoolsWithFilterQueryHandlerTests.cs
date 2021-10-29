using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools
{
    public class SelectLiquidityPoolsWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectLiquidityPoolsWithFilterQueryHandler _handler;

        public SelectLiquidityPoolsWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectLiquidityPoolsWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        // [Fact]
        // public async Task SelectAllPools_Success()
        // {
        //     const ulong marketId = 3;
        //
        //     var expectedEntity = new LiquidityPoolEntity
        //     {
        //         Id = 123454,
        //         SrcTokenId = 2,
        //         LpTokenId = 5,
        //         MarketId = marketId,
        //         Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
        //         CreatedBlock = 1,
        //         ModifiedBlock = 1
        //     };
        //
        //     var responseList = new[] { expectedEntity }.AsEnumerable();
        //
        //     var command = new SelectLiquidityPoolsWithFilterQuery(marketId);
        //
        //     _dbContext.Setup(db => db.ExecuteQueryAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync(responseList);
        //
        //     var results = await _handler.Handle(command, CancellationToken.None);
        //
        //     foreach (var result in results)
        //     {
        //         result.Id.Should().Be(expectedEntity.Id);
        //         result.SrcTokenId.Should().Be(expectedEntity.SrcTokenId);
        //         result.LpTokenId.Should().Be(expectedEntity.LpTokenId);
        //         result.MarketId.Should().Be(expectedEntity.MarketId);
        //         result.Address.Should().Be(expectedEntity.Address);
        //     }
        // }
    }
}
