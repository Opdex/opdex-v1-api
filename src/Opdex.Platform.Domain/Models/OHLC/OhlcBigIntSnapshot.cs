using Opdex.Platform.Common.Models.UInt;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.OHLC
{
    public class OhlcBigIntSnapshot
    {
        public OhlcBigIntSnapshot()
        {
            Open = UInt256.Zero;
            High = UInt256.Zero;
            Low = UInt256.Zero;
            Close = UInt256.Zero;
        }

        public OhlcBigIntSnapshot(IList<OhlcBigIntSnapshot> snapshots)
        {
            Open = snapshots.FirstOrDefault()?.Open ?? UInt256.Zero;
            High = snapshots.OrderByDescending(snapshot => snapshot.High).FirstOrDefault()?.High ?? UInt256.Zero;
            Low = snapshots.OrderBy(snapshot => snapshot.Low).FirstOrDefault()?.Low ?? UInt256.Zero;
            Close = snapshots.LastOrDefault()?.Close ?? UInt256.Zero;
        }

        public OhlcBigIntSnapshot(UInt256 open, UInt256 high, UInt256 low, UInt256 close)
        {
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }

        public UInt256 Open { get; private set; }
        public UInt256 High { get; private set; }
        public UInt256 Low { get; private set; }
        public UInt256 Close { get; private set; }

        internal void Update(UInt256 value, bool reset = false)
        {
            if (Open == UInt256.Zero || reset)
            {
                Open = value;
                High = value;
                Low = value;
                Close = value;

                return;
            }

            if (value > High)
            {
                High = value;
            }
            else if (value < Low)
            {
                Low = value;
            }

            Close = value;
        }
    }
}
