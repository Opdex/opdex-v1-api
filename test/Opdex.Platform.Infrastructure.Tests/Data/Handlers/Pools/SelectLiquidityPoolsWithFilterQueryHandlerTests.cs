using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Infrastructure;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools
{
    public class SelectAllLiquidityPoolsByMarketIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectAllLiquidityPoolsByMarketIdQueryHandler _handler;

        public SelectAllLiquidityPoolsByMarketIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectAllLiquidityPoolsByMarketIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectAllPools_Success()
        {
            const long marketId = 3;

            var expectedEntity = new LiquidityPoolEntity
            {
                Id = 123454,
                SrcTokenId = 2,
                LpTokenId = 5,
                MarketId = marketId,
                Address = "SomeAddress",
                CreatedBlock = 1,
                ModifiedBlock = 1
            };

            var responseList = new [] { expectedEntity }.AsEnumerable();

            var command = new SelectAllLiquidityPoolsByMarketIdQuery(marketId);

            _dbContext.Setup(db => db.ExecuteQueryAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(responseList));

            var results = await _handler.Handle(command, CancellationToken.None);

            foreach (var result in results)
            {
                result.Id.Should().Be(expectedEntity.Id);
                result.SrcTokenId.Should().Be(expectedEntity.SrcTokenId);
                result.LpTokenId.Should().Be(expectedEntity.LpTokenId);
                result.MarketId.Should().Be(expectedEntity.MarketId);
                result.Address.Should().Be(expectedEntity.Address);
            }
        }
    }
}