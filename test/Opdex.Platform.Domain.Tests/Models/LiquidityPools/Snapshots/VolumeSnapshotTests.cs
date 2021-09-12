using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots
{
    public class VolumeSnapshotTests
    {
        [Fact]
        public void CreateVolumeSnapshot_InvalidVolumeUsd_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const decimal volumeUsd = -1.00m;

            // Act
            void Act() => new VolumeSnapshot(123, 456, volumeUsd);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(volumeUsd)} must be greater or equal to 0.");
        }

        [Fact]
        public void CreateVolumeSnapshot_Success()
        {
            // Arrange
            const ulong volumeCrs = 123;
            UInt256 volumeSrc = 456;
            const decimal volumeUsd = 1.23m;

            // Act
            var snapshot = new VolumeSnapshot(volumeCrs, volumeSrc, volumeUsd);

            // Assert
            snapshot.Crs.Should().Be(volumeCrs);
            snapshot.Src.Should().Be(volumeSrc);
            snapshot.Usd.Should().Be(volumeUsd);
        }

        [Fact]
        public void CreateVolumeSnapshot_Default_Success()
        {
            // Arrange
            // Act
            var snapshot = new VolumeSnapshot();

            // Assert
            snapshot.Crs.Should().Be(0ul);
            snapshot.Src.Should().Be(UInt256.Zero);
            snapshot.Usd.Should().Be(0.00m);
        }
    }
}
