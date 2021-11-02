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
            void Act() => new LiquidityPool(null, "ETH-CRS", 1, 2, 3, 4);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Liquidity pool address must be provided");
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void CreateNewLiquidityPool_InvalidName_ThrowsArgumentNullException(string name)
        {
            // Arrange
            // Act
            void Act() => new LiquidityPool("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", name, 1, 2, 3, 4);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Name must be provided");
        }

        [Fact]
        public void CreateNewLiquidityPool_InvalidSrcTokenId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new LiquidityPool("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "ETH-CRS", 0, 2, 3, 4);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("SRC token id must be greater than zero.");
        }

        [Fact]
        public void CreateNewLiquidityPool_InvalidLpTokenId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new LiquidityPool("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "ETH-CRS", 1, 0, 3, 4);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Liquidity pool token id must be greater than zero.");
        }

        [Fact]
        public void CreateNewLiquidityPool_InvalidMarketId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new LiquidityPool("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "ETH-CRS", 1, 2, 0, 4);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Market id must be greater than zero.");
        }


        [Fact]
        public void CreateNewLiquidityPool_Success()
        {
            Address address = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
            const string name = "ETH-CRS";
            const ulong srcId = 2;
            const ulong lptId = 4;
            const ulong marketId = 1;
            const ulong createdBlock = 3;

            var pool = new LiquidityPool(address, name, srcId, lptId, marketId, createdBlock);

            pool.Id.Should().Be(0);
            pool.Name.Should().Be(name);
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
            const string name = "ETH-CRS";
            const ulong id = 999;
            const ulong srcId = 2;
            const ulong lptId = 4;
            const ulong marketId = 1;
            const ulong createdBlock = 3;
            const ulong modifiedBlock = 10;

            var pool = new LiquidityPool(id, address, name, srcId, lptId, marketId, createdBlock, modifiedBlock);

            pool.Id.Should().Be(id);
            pool.Name.Should().Be(name);
            pool.Address.Should().Be(address);
            pool.SrcTokenId.Should().Be(srcId);
            pool.LpTokenId.Should().Be(lptId);
            pool.CreatedBlock.Should().Be(createdBlock);
            pool.ModifiedBlock.Should().Be(modifiedBlock);
        }
    }
}
