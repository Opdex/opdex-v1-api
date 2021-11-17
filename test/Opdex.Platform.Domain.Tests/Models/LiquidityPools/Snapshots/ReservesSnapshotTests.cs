using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.OHLC;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots
{
    public class ReservesSnapshotTests
    {
        [Fact]
        public void CreateReservesSnapshot_Success()
        {
            // Arrange
            const ulong reserveCrs = 123;
            UInt256 reserveSrc = 321;
            var reserveUsd = new OhlcDecimalSnapshot(1.23m, 2.0m, 0.02m, 1.56m);

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
            snapshot.Usd.Should().BeEquivalentTo(new OhlcDecimalSnapshot());
        }
    }
}
