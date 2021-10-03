using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.LiquidityPools;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools
{
    public class LiquidityPoolTests
    {
        [Fact]
        public void CreateNewLiquidityPool_InvalidAddress_ThrowsArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new LiquidityPool(null, 1, 2, 3, 4);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Liquidity pool address must be provided");
        }

        [Fact]
        public void CreateNewLiquidityPool_InvalidSrcTokenId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new LiquidityPool("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 0, 2, 3, 4);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("SRC token id must be greater than zero.");
        }

        [Fact]
        public void CreateNewLiquidityPool_InvalidLpTokenId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new LiquidityPool("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 1, 0, 3, 4);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Liquidity pool token id must be greater than zero.");
        }

        [Fact]
        public void CreateNewLiquidityPool_InvalidMarketId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new LiquidityPool("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 1, 2, 0, 4);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Market id must be greater than zero.");
        }


        [Fact]
        public void CreateNewLiquidityPool_Success()
        {
            Address address = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
            const ulong srcId = 2;
            const ulong lptId = 4;
            const ulong marketId = 1;
            const ulong createdBlock = 3;

            var pool = new LiquidityPool(address, srcId, lptId, marketId, createdBlock);

            pool.Id.Should().Be(0);
            pool.Address.Should().Be(address);
            pool.SrcTokenId.Should().Be(srcId);
            pool.LpTokenId.Should().Be(lptId);
            pool.CreatedBlock.Should().Be(createdBlock);
            pool.ModifiedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void CreateExistingLiquidityPool_Success()
        {
            Address address = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
            const ulong id = 999;
            const ulong srcId = 2;
            const ulong lptId = 4;
            const ulong marketId = 1;
            const ulong createdBlock = 3;
            const ulong modifiedBlock = 10;

            var pool = new LiquidityPool(id, address, srcId, lptId, marketId, createdBlock, modifiedBlock);

            pool.Id.Should().Be(id);
            pool.Address.Should().Be(address);
            pool.SrcTokenId.Should().Be(srcId);
            pool.LpTokenId.Should().Be(lptId);
            pool.CreatedBlock.Should().Be(createdBlock);
            pool.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
