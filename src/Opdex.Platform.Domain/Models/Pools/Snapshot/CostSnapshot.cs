using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshot
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
            CrsPerSrc = crsPerSrc ?? throw new ArgumentNullException(nameof(crsPerSrc));
            SrcPerCrs = srcPerCrs ?? throw new ArgumentNullException(nameof(srcPerCrs));
        }

        public OhlcBigIntSnapshot CrsPerSrc { get; }
        public OhlcBigIntSnapshot SrcPerCrs { get; }

        internal void SetCost(ulong reserveCrs, string reserveSrc, ulong srcSats, bool reset)
        {
            CrsPerSrc.Update(reserveCrs.Token0PerToken1(reserveSrc, srcSats), reset);
            SrcPerCrs.Update(reserveSrc.Token0PerToken1(reserveCrs, TokenConstants.Cirrus.Sats), reset);
        }
    }
}