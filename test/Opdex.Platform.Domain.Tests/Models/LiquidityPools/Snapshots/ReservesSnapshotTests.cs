using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots
{
    public class ReservesSnapshotTests
    {
        [Fact]
        public void CreateReservesSnapshot_Success()
        {
            // Arrange
            var reserveSrc = new Ohlc<UInt256>(321, 321, 321, 321);
            var reserveUsd = new Ohlc<decimal>(1.23m, 2.0m, 0.02m, 1.56m);
            var reserveCrs = new Ohlc<ulong>(123, 123, 123, 123);

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
            snapshot.Crs.Should().BeEquivalentTo(new Ohlc<ulong>());
            snapshot.Src.Should().BeEquivalentTo(new Ohlc<UInt256>());
            snapshot.Usd.Should().BeEquivalentTo(new Ohlc<decimal>());
        }
    }
}
