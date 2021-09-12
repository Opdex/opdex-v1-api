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

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools
{
    public class SelectLiquidityPoolByLpTokenIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectLiquidityPoolByLpTokenIdQueryHandler _handler;

        public SelectLiquidityPoolByLpTokenIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectLiquidityPoolByLpTokenIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectLiquidityByLpTokenId_Success()
        {
            const long lpTokenId = 99;

            var expectedEntity = new LiquidityPoolEntity
            {
                Id = 123454,
                SrcTokenId = 1235,
                LpTokenId = lpTokenId,
                MarketId = 1,
                Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u",
                CreatedBlock = 1,
                ModifiedBlock = 1
            };

            var command = new SelectLiquidityPoolByLpTokenIdQuery(lpTokenId);

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
        public void SelectLiquidityByLpTokenId_Throws_NotFoundException()
        {
            const long lpTokenId = 99;

            var command = new SelectLiquidityPoolByLpTokenIdQuery(lpTokenId);

            _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<LiquidityPoolEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(LiquidityPool)} not found.");
        }

        [Fact]
        public async Task SelectLiquidityByLpTokenId_ReturnsNull()
        {
            const long lpTokenId = 99;
            const bool findOrThrow = false;

            var command = new SelectLiquidityPoolByLpTokenIdQuery(lpTokenId, findOrThrow);

            _dbContext.Setup(db => db.ExecuteFindAsync<LiquidityPoolEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<LiquidityPoolEntity>(null));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
