using FluentAssertions;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.OHLC;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots
{
    public class CostSnapshotTests
    {
        [Fact]
        public void CreateCostSnapshot_InvalidCrsPerSrc_ThrowsArgumentNullException()
        {
            // Arrange
            OhlcBigIntSnapshot crsPerSrc = null;
            var srcPerCrs = new OhlcBigIntSnapshot();

            // Act
            void Act() => new CostSnapshot(crsPerSrc, srcPerCrs);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(crsPerSrc)} cannot be null.");
        }

        [Fact]
        public void CreateCostSnapshot_InvalidCostUsd_ThrowsArgumentNullException()
        {
            // Arrange
            var crsPerSrc = new OhlcBigIntSnapshot();
            OhlcBigIntSnapshot srcPerCrs = null;

            // Act
            void Act() => new CostSnapshot(crsPerSrc, srcPerCrs);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(srcPerCrs)} cannot be null.");
        }

        [Fact]
        public void CreateCostSnapshot_Success()
        {
            // Arrange
            var crsPerSrc = new OhlcBigIntSnapshot();
            var srcPerCrs = new OhlcBigIntSnapshot();

            // Act
            var snapshot = new CostSnapshot(crsPerSrc, srcPerCrs);

            // Assert
            snapshot.CrsPerSrc.Should().NotBeNull();
            snapshot.SrcPerCrs.Should().NotBeNull();
        }

        [Fact]
        public void CreateCostSnapshot_Default_Success()
        {
            // Arrange
            // Act
            var snapshot = new CostSnapshot();

            // Assert
            snapshot.CrsPerSrc.Should().NotBeNull();
            snapshot.SrcPerCrs.Should().NotBeNull();
        }
    }
}
