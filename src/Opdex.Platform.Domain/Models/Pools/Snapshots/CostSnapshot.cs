using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.OHLC;

namespace Opdex.Platform.Domain.Models.Pools.Snapshots
{
    public class CostSnapshot
    {
        public CostSnapshot()
        {
            CrsPerSrc = new OhlcBigIntSnapshot();
            SrcPerCrs = new OhlcBigIntSnapshot();
        }

        public CostSnapshot(OhlcBigIntSnapshot crsPerSrc, OhlcBigIntSnapshot srcPerCrs)
        {
            CrsPerSrc = crsPerSrc ?? throw new ArgumentNullException(nameof(crsPerSrc), $"{nameof(crsPerSrc)} cannot be null.");
            SrcPerCrs = srcPerCrs ?? throw new ArgumentNullException(nameof(srcPerCrs), $"{nameof(srcPerCrs)} cannot be null.");
        }

        public OhlcBigIntSnapshot CrsPerSrc { get; }
        public OhlcBigIntSnapshot SrcPerCrs { get; }

        internal void SetCost(ulong reserveCrs, string reserveSrc, ulong srcSats, bool reset = false)
        {
            CrsPerSrc.Update(reserveCrs.Token0PerToken1(reserveSrc, srcSats), reset);
            SrcPerCrs.Update(reserveSrc.Token0PerToken1(reserveCrs, TokenConstants.Cirrus.Sats), reset);
        }
    }
}