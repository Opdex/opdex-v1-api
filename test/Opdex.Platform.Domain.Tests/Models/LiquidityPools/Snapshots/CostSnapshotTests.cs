using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
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
            var srcPerCrs = new Ohlc<UInt256>();

            // Act
            void Act() => new CostSnapshot(null, srcPerCrs);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("crsPerSrc cannot be null.");
        }

        [Fact]
        public void CreateCostSnapshot_InvalidCostUsd_ThrowsArgumentNullException()
        {
            // Arrange
            var crsPerSrc = new Ohlc<UInt256>();

            // Act
            void Act() => new CostSnapshot(crsPerSrc, null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("srcPerCrs cannot be null.");
        }

        [Fact]
        public void CreateCostSnapshot_Success()
        {
            // Arrange
            var crsPerSrc = new Ohlc<UInt256>();
            var srcPerCrs = new Ohlc<UInt256>();

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
