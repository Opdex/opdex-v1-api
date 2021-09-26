using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.OHLC;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots
{
    public class CostSnapshot
    {
        public CostSnapshot()
        {
            CrsPerSrc = new OhlcBigIntSnapshot();
            SrcPerCrs = new OhlcBigIntSnapshot();
        }

        public CostSnapshot(IList<CostSnapshot> snapshots)
        {
            CrsPerSrc = new OhlcBigIntSnapshot(snapshots.Select(snapshot => snapshot.CrsPerSrc).ToList());
            SrcPerCrs = new OhlcBigIntSnapshot(snapshots.Select(snapshot => snapshot.SrcPerCrs).ToList());
        }

        public CostSnapshot(OhlcBigIntSnapshot crsPerSrc, OhlcBigIntSnapshot srcPerCrs)
        {
            CrsPerSrc = crsPerSrc ?? throw new ArgumentNullException(nameof(crsPerSrc), $"{nameof(crsPerSrc)} cannot be null.");
            SrcPerCrs = srcPerCrs ?? throw new ArgumentNullException(nameof(srcPerCrs), $"{nameof(srcPerCrs)} cannot be null.");
        }

        public OhlcBigIntSnapshot CrsPerSrc { get; }
        public OhlcBigIntSnapshot SrcPerCrs { get; }

        internal void SetCost(ulong reserveCrs, UInt256 reserveSrc, ulong srcSats, bool reset = false)
        {
            CrsPerSrc.Update(reserveCrs.Token0PerToken1(reserveSrc, srcSats), reset);
            SrcPerCrs.Update(reserveSrc.Token0PerToken1(reserveCrs, TokenConstants.Cirrus.Sats), reset);
        }
    }
}
