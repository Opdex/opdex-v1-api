using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools
{
    public class SelectMiningPoolByLiquidityPoolIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMiningPoolByLiquidityPoolIdQueryHandler _handler;

        public SelectMiningPoolByLiquidityPoolIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMiningPoolByLiquidityPoolIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectMiningPoolByLiquidityPoolId_Success()
        {
            const long lpId = 99;

            var expectedEntity = new MiningPoolEntity
            {
                Id = 123454,
                LiquidityPoolId = lpId,
                Address = "SomeAddress",
                MiningPeriodEndBlock = 4,
                RewardPerBlock = "10",
                RewardPerLpt = "1",
                CreatedBlock = 1,
                ModifiedBlock = 2
            };

            var command = new SelectMiningPoolByLiquidityPoolIdQuery(lpId);

            _dbContext.Setup(db => db.ExecuteFindAsync<MiningPoolEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedEntity));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedEntity.Id);
            result.LiquidityPoolId.Should().Be(expectedEntity.LiquidityPoolId);
            result.MiningPeriodEndBlock.Should().Be(expectedEntity.MiningPeriodEndBlock);
            result.RewardPerBlock.Should().Be(expectedEntity.RewardPerBlock);
            result.RewardPerLpt.Should().Be(expectedEntity.RewardPerLpt);
            result.Address.Should().Be(expectedEntity.Address);
            result.CreatedBlock.Should().Be(expectedEntity.CreatedBlock);
            result.ModifiedBlock.Should().Be(expectedEntity.ModifiedBlock);
        }

        [Fact]
        public void SelectMiningPoolByLiquidityPoolId_Throws_NotFoundException()
        {
            const long lpId = 99;

            var command = new SelectMiningPoolByLiquidityPoolIdQuery(lpId);

            _dbContext.Setup(db => db.ExecuteFindAsync<MiningPoolEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MiningPoolEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(MiningPool)} not found.");
        }

        [Fact]
        public async Task SelectMiningPoolByLiquidityPoolId_ReturnsNull()
        {
            const long lpId = 99;
            const bool findOrThrow = false;

            var command = new SelectMiningPoolByLiquidityPoolIdQuery(lpId, findOrThrow);

            _dbContext.Setup(db => db.ExecuteFindAsync<MiningPoolEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MiningPoolEntity>(null));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
