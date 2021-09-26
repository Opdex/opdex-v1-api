using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.OHLC
{
    public class OhlcDecimalSnapshot
    {
        private const decimal DefaultValue = 0.00000000m;

        public OhlcDecimalSnapshot()
        {
            Open = DefaultValue;
            High = DefaultValue;
            Low = DefaultValue;
            Close = DefaultValue;
        }

        public OhlcDecimalSnapshot(IList<OhlcDecimalSnapshot> snapshots)
        {
            Open = snapshots.FirstOrDefault()?.Open ?? DefaultValue;
            High = snapshots.OrderByDescending(snapshot => snapshot.High).FirstOrDefault()?.High ?? DefaultValue;
            Low = snapshots.OrderBy(snapshot => snapshot.Low).FirstOrDefault()?.Low ?? DefaultValue;
            Close = snapshots.LastOrDefault()?.Close ?? DefaultValue;
        }

        public OhlcDecimalSnapshot(decimal open, decimal high, decimal low, decimal close)
        {
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }

        public decimal Open { get; private set; }
        public decimal High { get; private set; }
        public decimal Low { get; private set; }
        public decimal Close { get; private set; }

        internal void Update(decimal value, bool reset = false)
        {
            if (Open == DefaultValue || reset)
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
