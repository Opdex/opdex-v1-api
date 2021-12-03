using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;

public class CostSnapshot
{
    public CostSnapshot()
    {
        CrsPerSrc = new Ohlc<UInt256>();
        SrcPerCrs = new Ohlc<UInt256>();
    }

    public CostSnapshot(IList<CostSnapshot> snapshots)
    {
        CrsPerSrc = new Ohlc<UInt256>(snapshots.Select(snapshot => snapshot.CrsPerSrc).ToList());
        SrcPerCrs = new Ohlc<UInt256>(snapshots.Select(snapshot => snapshot.SrcPerCrs).ToList());
    }

    public CostSnapshot(Ohlc<UInt256> crsPerSrc, Ohlc<UInt256> srcPerCrs)
    {
        CrsPerSrc = crsPerSrc ?? throw new ArgumentNullException(nameof(crsPerSrc), $"{nameof(crsPerSrc)} cannot be null.");
        SrcPerCrs = srcPerCrs ?? throw new ArgumentNullException(nameof(srcPerCrs), $"{nameof(srcPerCrs)} cannot be null.");
    }

    public Ohlc<UInt256> CrsPerSrc { get; }
    public Ohlc<UInt256> SrcPerCrs { get; }

    /// <summary>
    /// Updates the cost snapshot with new OHLC close values plus low/high when applicable.
    /// </summary>
    /// <param name="reserveCrs">Amount of CRS in reserves.</param>
    /// <param name="reserveSrc">Amount of SRC in reserves.</param>
    /// <param name="srcSats">Amount of sats per SRC token.</param>
    internal void Update(ulong reserveCrs, UInt256 reserveSrc, ulong srcSats)
    {
        Update(reserveCrs, reserveSrc, srcSats, false);
    }

    /// <summary>
    /// Refreshes the cost snapshot to use new OHLC values based in input values.
    /// </summary>
    /// <param name="reserveCrs">Amount of CRS in reserves.</param>
    /// <param name="reserveSrc">Amount of SRC in reserves.</param>
    /// <param name="srcSats">Amount of sats per SRC token.</param>
    internal void Refresh(ulong reserveCrs, UInt256 reserveSrc, ulong srcSats)
    {
        Update(reserveCrs, reserveSrc, srcSats, true);
    }

    private void Update(ulong reserveCrs, UInt256 reserveSrc, ulong srcSats, bool refresh)
    {
        var crsPerSrc = reserveCrs.Token0PerToken1(reserveSrc, srcSats);
        var srcPerCrs = reserveSrc.Token0PerToken1(reserveCrs, TokenConstants.Cirrus.Sats);

        if (refresh)
        {
            CrsPerSrc.Refresh(crsPerSrc);
            SrcPerCrs.Refresh(srcPerCrs);
        }
        else
        {
            CrsPerSrc.Update(crsPerSrc);
            SrcPerCrs.Update(srcPerCrs);
        }
    }
}