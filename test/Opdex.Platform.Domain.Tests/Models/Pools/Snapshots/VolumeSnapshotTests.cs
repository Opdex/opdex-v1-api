using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Pools.Snapshots
{
    public class VolumeSnapshotTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("1.234")]
        [InlineData(null)]
        public void CreateVolumeSnapshot_InvalidVolumeSrc_ThrowsArgumentOutOfRangeException(string volumeSrc)
        {
            // Arrange
            // Act
            void Act() => new VolumeSnapshot(123, volumeSrc, 1.00m);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(volumeSrc)} must be a numeric value.");
        }

        [Fact]
        public void CreateVolumeSnapshot_InvalidVolumeUsd_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const decimal volumeUsd = -1.00m;

            // Act
            void Act() => new VolumeSnapshot(123, "456", volumeUsd);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(volumeUsd)} must be greater or equal to 0.");
        }

        [Fact]
        public void CreateVolumeSnapshot_Success()
        {
            // Arrange
            const ulong volumeCrs = 123;
            const string volumeSrc = "456";
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
            snapshot.Src.Should().Be("0");
            snapshot.Usd.Should().Be(0.00m);
        }
    }
}
