using System;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshot
{
    public class CostSnapshot
    {
        public CostSnapshot()
        {
            CrsPerSrc = new OhlcSnapshot();
            SrcPerCrs = new OhlcSnapshot();
        }

        public CostSnapshot(OhlcSnapshot crsPerSrc, OhlcSnapshot srcPerCrs)
        {
            CrsPerSrc = crsPerSrc ?? throw new ArgumentNullException(nameof(crsPerSrc));
            SrcPerCrs = srcPerCrs ?? throw new ArgumentNullException(nameof(srcPerCrs));
        }

        public OhlcSnapshot CrsPerSrc { get; }
        public OhlcSnapshot SrcPerCrs { get; }

        internal void SetCost(ReservesLog log, ulong srcSats)
        {
            CrsPerSrc.Update(log.CrsPerSrc(srcSats));
            SrcPerCrs.Update(log.SrcPerCrs());
        }
    }
}