using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System.Collections.Generic;
using System.Dynamic;
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

        [Fact]
        public void CreateReservesSnapshot_FromSnapshots_Success()
        {
            // Arrange
            var snapshots = new List<ReservesSnapshot>
            {
                new ReservesSnapshot(new Ohlc<ulong>(10, 100, 1, 50), new Ohlc<UInt256>(20, 200, 2, 100), new Ohlc<decimal>(10, 100, 1, 50)),
                new ReservesSnapshot(new Ohlc<ulong>(1, 10, 1, 10), new Ohlc<UInt256>(2, 20, 2, 20), new Ohlc<decimal>(1, 10, 1, 10)),
                new ReservesSnapshot(new Ohlc<ulong>(50, 150, 45, 100), new Ohlc<UInt256>(100, 300, 90, 200), new Ohlc<decimal>(50, 150, 45, 100)),
            };

            var expectedCrsReserves = new Ohlc<ulong>(10, 150, 1, 100);
            var expectedSrcReserves = new Ohlc<UInt256>(20, 300, 2, 200);
            var expectedUsdReserves = new Ohlc<decimal>(10, 150, 1, 100);

            // Act
            var snapshot = new ReservesSnapshot(snapshots);

            // Assert
            snapshot.Crs.Should().BeEquivalentTo(expectedCrsReserves);
            snapshot.Src.Should().BeEquivalentTo(expectedSrcReserves);
            snapshot.Usd.Should().BeEquivalentTo(expectedUsdReserves);
        }

        [Fact]
        public void Update_ReservesLog_Success()
        {
            // Arrange
            const decimal crsUsd = 1m;
            var snapshot = new ReservesSnapshot(new Ohlc<ulong>(10, 50, 1, 50),
                                                new Ohlc<UInt256>(20, 200, 2, 100),
                                                new Ohlc<decimal>(.0000002m, .000001m, .00000002m, .000001m));

            dynamic txLog = new ExpandoObject();
            txLog.reserveCrs = 60ul;
            txLog.reserveSrc = "90";
            var reservesLog = new ReservesLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 0);

            var expectedCrs = new Ohlc<ulong>(10, 60, 1, 60); // $1 per token * 100 sats closing staked
            var expectedSrc = new Ohlc<UInt256>(20, 200, 2, 90);
            var expectedUsd = new Ohlc<decimal>(.0000002m, .0000012m, .00000002m, .0000012m);

            // Act
            snapshot.Update(reservesLog, crsUsd);

            // Assert
            snapshot.Crs.Should().BeEquivalentTo(expectedCrs);
            snapshot.Src.Should().BeEquivalentTo(expectedSrc);
            snapshot.Usd.Should().BeEquivalentTo(expectedUsd);
        }

        [Fact]
        public void Update_StaleUsd_Success()
        {
            // Arrange
            const decimal crsUsd = 1.5m;

            // priced at $1
            var snapshot = new ReservesSnapshot(new Ohlc<ulong>(10, 50, 1, 50),
                                                new Ohlc<UInt256>(20, 200, 2, 100),
                                                new Ohlc<decimal>(.0000002m, .000001m, .00000002m, .000001m));

            // new price
            var expectedSnapshot = new ReservesSnapshot(new Ohlc<ulong>(10, 50, 1, 50),
                                                        new Ohlc<UInt256>(20, 200, 2, 100),
                                                        new Ohlc<decimal>(.0000002m, .0000015m, .00000002m, .0000015m));

            // Act
            snapshot.Update(crsUsd);

            // Assert
            snapshot.Should().BeEquivalentTo(expectedSnapshot);
        }

        [Fact]
        public void Refresh_Snapshot_Success()
        {
            // Arrange
            const decimal crsUsd = 1.5m;

            // Priced at $1
            var snapshot = new ReservesSnapshot(new Ohlc<ulong>(10, 50, 1, 50),
                                                new Ohlc<UInt256>(20, 200, 2, 100),
                                                new Ohlc<decimal>(.0000002m, .000001m, .00000002m, .000001m));

            // new price
            var expectedSnapshot = new ReservesSnapshot(new Ohlc<ulong>(50, 50, 50, 50),
                                                        new Ohlc<UInt256>(100, 100, 100, 100),
                                                        new Ohlc<decimal>(.0000015m, .0000015m, .0000015m, .0000015m));
            // Act
            snapshot.Refresh(crsUsd);

            // Assert
            snapshot.Should().BeEquivalentTo(expectedSnapshot);
        }
    }
}
