using AutoMapper;
using FluentAssertions;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests
{
    public class PlatformInfrastructureMapperProfileTests
    {
        private readonly IMapper _mapper;

        public PlatformInfrastructureMapperProfileTests()
        {
            _mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
        }

        #region Liquidity Pools

        [Fact]
        public void From_LiquidityPoolSummary_To_LiquidityPoolSummaryEntity()
        {
            // Arrange
            var model = new LiquidityPoolSummary(100, 1, 2.00m, 3.00m, 4, 5, 6, 7, 8);

            // Act
            var entity = _mapper.Map<LiquidityPoolSummaryEntity>(model);

            // Assert
            entity.Id.Should().Be(model.Id);
            entity.LiquidityPoolId.Should().Be(model.LiquidityPoolId);
            entity.Liquidity.Should().Be(model.Liquidity);
            entity.Volume.Should().Be(model.Volume);
            entity.StakingWeight.Should().Be(model.StakingWeight);
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
                Liquidity = 2.00m,
                Volume = 3.00m,
                StakingWeight = 4,
                LockedCrs = 5,
                LockedSrc = 6,
                CreatedBlock = 7,
                ModifiedBlock = 8
            };

            // Act
            var model = _mapper.Map<LiquidityPoolSummary>(entity);

            // Assert
            model.Id.Should().Be(entity.Id);
            model.LiquidityPoolId.Should().Be(entity.LiquidityPoolId);
            model.Liquidity.Should().Be(entity.Liquidity);
            model.Volume.Should().Be(entity.Volume);
            model.StakingWeight.Should().Be(entity.StakingWeight);
            model.LockedCrs.Should().Be(entity.LockedCrs);
            model.LockedSrc.Should().Be(entity.LockedSrc);
            model.CreatedBlock.Should().Be(entity.CreatedBlock);
            model.ModifiedBlock.Should().Be(entity.ModifiedBlock);
        }

        #endregion
    }
}
