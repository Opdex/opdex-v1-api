using FluentAssertions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Mappers
{
    public class LiquidityPoolsPlatformInfrastructureMapperProfile : PlatformInfrastructureMapperProfileTests
    {
        [Fact]
        public void From_LiquidityPoolSummary_To_LiquidityPoolSummaryEntity()
        {
            // Arrange
            var model = new LiquidityPoolSummary(100, 1, 2.00m, 4.5m, 3.00m, 4, 6.5m, 5, 6, 7, 8);

            // Act
            var entity = _mapper.Map<LiquidityPoolSummaryEntity>(model);

            // Assert
            entity.Id.Should().Be(model.Id);
            entity.LiquidityPoolId.Should().Be(model.LiquidityPoolId);
            entity.LiquidityUsd.Should().Be(model.LiquidityUsd);
            entity.DailyLiquidityUsdChangePercent.Should().Be(model.DailyLiquidityUsdChangePercent);
            entity.VolumeUsd.Should().Be(model.VolumeUsd);
            entity.StakingWeight.Should().Be(model.StakingWeight);
            entity.DailyStakingWeightChangePercent.Should().Be(model.DailyStakingWeightChangePercent);
            entity.LockedCrs.Should().Be(model.LockedCrs);
            entity.LockedSrc.Should().Be(model.LockedSrc);
            entity.CreatedBlock.Should().Be(model.CreatedBlock);
            entity.ModifiedBlock.Should().Be(model.ModifiedBlock);
        }

        [Fact]
        public void From_LiquidityPoolSummaryEntity_To_LiquidityPoolSummary()
        {
            // Arrange
            var entity = new LiquidityPoolSummaryEntity
            {
                Id = 100,
                LiquidityPoolId = 1,
                LiquidityUsd = 2.00m,
                DailyLiquidityUsdChangePercent = 2.50m,
                VolumeUsd = 3.00m,
                StakingWeight = 4,
                DailyStakingWeightChangePercent = 4.5m,
                LockedCrs = 5,
                LockedSrc = 6,
                CreatedBlock = 7,
                ModifiedBlock = 8
            };

            // Act
            var model = _mapper.Map<LiquidityPoolSummary>(entity);

            // Assert
            entity.Id.Should().Be(model.Id);
            entity.LiquidityPoolId.Should().Be(model.LiquidityPoolId);
            entity.LiquidityUsd.Should().Be(model.LiquidityUsd);
            entity.DailyLiquidityUsdChangePercent.Should().Be(model.DailyLiquidityUsdChangePercent);
            entity.VolumeUsd.Should().Be(model.VolumeUsd);
            entity.StakingWeight.Should().Be(model.StakingWeight);
            entity.DailyStakingWeightChangePercent.Should().Be(model.DailyStakingWeightChangePercent);
            entity.LockedCrs.Should().Be(model.LockedCrs);
            entity.LockedSrc.Should().Be(model.LockedSrc);
            entity.CreatedBlock.Should().Be(model.CreatedBlock);
            entity.ModifiedBlock.Should().Be(model.ModifiedBlock);
        }
    }
}
