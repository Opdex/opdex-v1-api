using FluentAssertions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.LiquidityPools.Snapshots;

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

    [Fact]
    public void CreateVolumeSnapshot_FromSnapshots_Success()
    {
        // Arrange
        var snapshots = new List<VolumeSnapshot>
        {
            new VolumeSnapshot(50, 100, 50),
            new VolumeSnapshot(10, 20, 10),
            new VolumeSnapshot(100, 200, 100),
        };

        const ulong expectedCrs = 160;
        UInt256 expectedSrc = 320;
        const decimal expectedUsd = 160;

        // Act
        var snapshot = new VolumeSnapshot(snapshots);

        // Assert
        snapshot.Crs.Should().Be(expectedCrs);
        snapshot.Src.Should().BeEquivalentTo(expectedSrc);
        snapshot.Usd.Should().Be(expectedUsd);
    }

    [Fact]
    public void Update_SrcSwap_Success()
    {
        // Arrange
        const decimal crsUsd = 100m;
        const decimal srcUsd = 50m;
        const ulong srcSats = 100_000_000;
        var snapshot = new VolumeSnapshot(100, 200, .0001m);

        dynamic txLog = new ExpandoObject();
        txLog.sender = "PDVGVAgvj5M2p2uVqojah5kcXzHiBtV8LV";
        txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
        txLog.amountCrsIn = 0ul;
        txLog.amountSrcIn = "20";
        txLog.amountCrsOut = 10ul;
        txLog.amountSrcOut = "0";
        var swapLog = new SwapLog(txLog, "Pvj5M2p2uVqojah5g8LVkcXzHiBtVDVGVA", 5);

        const ulong expectedCrs = 110;
        UInt256 expectedSrc = 220;
        const decimal expectedUsd = .00011m;

        // Act
        snapshot.Update(swapLog, crsUsd, srcUsd, srcSats);

        // Assert
        snapshot.Crs.Should().Be(expectedCrs);
        snapshot.Src.Should().Be(expectedSrc);
        snapshot.Usd.Should().Be(expectedUsd);
    }
}