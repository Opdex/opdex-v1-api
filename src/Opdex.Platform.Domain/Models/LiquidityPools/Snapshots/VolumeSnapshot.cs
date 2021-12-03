using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;

public class VolumeSnapshot
{
    public VolumeSnapshot()
    {
        Crs = 0;
        Src = 0;
        Usd = 0.00000000m;
    }

    public VolumeSnapshot(IList<VolumeSnapshot> snapshots)
    {
        Crs = snapshots.Aggregate(0UL, (a,c) => a + c.Crs);
        Src = snapshots.Aggregate(UInt256.Zero, (a,c) => a + c.Src);
        Usd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Usd);
    }

    public VolumeSnapshot(ulong volumeCrs, UInt256 volumeSrc, decimal volumeUsd)
    {
        if (volumeUsd < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(volumeUsd), $"{nameof(volumeUsd)} must be greater or equal to 0.");
        }

        Crs = volumeCrs;
        Src = volumeSrc;
        Usd = volumeUsd;
    }

    public ulong Crs { get; private set; }
    public UInt256 Src { get; private set; }
    public decimal Usd { get; private set; }

    /// <summary>
    /// Update the snapshot volume with a Swap Log. Track CRS and SRC volume in addition to USD volume
    /// by the amount in token and it's USD value.
    /// </summary>
    /// <param name="log">The SwapLog to track the volume of.</param>
    /// <param name="crsUsd">The CRS USD price at the time just prior to the transaction.</param>
    /// <param name="srcUsd">The SRC USD price at the time just prior to the transaction</param>
    /// <param name="srcSats">The number of sats within the SRC token.</param>
    internal void Update(SwapLog log, decimal crsUsd, decimal srcUsd, ulong srcSats)
    {
        Crs += log.AmountCrsIn + log.AmountCrsOut;

        var volumeSrc = log.AmountSrcIn + log.AmountSrcOut;
        Src += volumeSrc;

        var crsVolume = MathExtensions.TotalFiat(log.AmountCrsIn, crsUsd, TokenConstants.Cirrus.Sats);
        var srcVolume = MathExtensions.TotalFiat(log.AmountSrcIn, srcUsd, srcSats);

        Usd += Math.Round(crsVolume + srcVolume, 8, MidpointRounding.AwayFromZero);
    }
}