using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots
{
    public class ReservesSnapshotTests
    {
        [Fact]
        public void CreateReservesSnapshot_InvalidReserveUsd_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const decimal reserveUsd = -1.00m;

            // Act
            void Act() => new ReservesSnapshot(12345, 9876, reserveUsd);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(reserveUsd)} must be greater or equal to 0.");
        }

        [Fact]
        public void CreateReservesSnapshot_Success()
        {
            // Arrange
            const ulong reserveCrs = 123;
            UInt256 reserveSrc = 321;
            const decimal reserveUsd = 1.23m;

            // Act
            var snapshot = new ReservesSnapshot(reserveCrs, reserveSrc, reserveUsd);

            // Assert
            snapshot.Crs.Should().Be(reserveCrs);
            snapshot.Src.Should().Be(reserveSrc);
            snapshot.Usd.Should().Be(reserveUsd);
        }

        [Fact]
        public void CreateReservesSnapshot_Default_Success()
        {
            // Arrange
            // Act
            var snapshot = new ReservesSnapshot();

            // Assert
            snapshot.Crs.Should().Be(0ul);
            snapshot.Src.Should().Be(UInt256.Zero);
            snapshot.Usd.Should().Be(0.00m);
        }
    }
}
