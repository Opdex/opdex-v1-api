using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models.OHLC
{
    public class OhlcBigIntSnapshot
    {
        private const string DefaultValue = "0";

        public OhlcBigIntSnapshot()
        {
            Open = DefaultValue;
            High = DefaultValue;
            Low = DefaultValue;
            Close = DefaultValue;
        }

        public OhlcBigIntSnapshot(string open, string high, string low, string close)
        {
            if (!open.IsNumeric())
            {
                throw new ArgumentNullException(nameof(open), $"{nameof(open)} must be a numeric value.");
            }

            if (!high.IsNumeric())
            {
                throw new ArgumentNullException(nameof(high), $"{nameof(high)} must be a numeric value.");
            }

            if (!low.IsNumeric())
            {
                throw new ArgumentNullException(nameof(low), $"{nameof(low)} must be a numeric value.");
            }

            if (!close.IsNumeric())
            {
                throw new ArgumentNullException(nameof(close), $"{nameof(close)} must be a numeric value.");
            }

            Open = open;
            High = high;
            Low = low;
            Close = close;
        }

        public string Open { get; private set; }
        public string High { get; private set; }
        public string Low { get; private set; }
        public string Close { get; private set; }

        internal void Update(string value, bool reset)
        {
            if (Open == DefaultValue)
            {
                Open = value;
                High = value;
                Low = value;
                Close = value;

                return;
            }

            if (reset)
            {
                Open = Close;
                High = Open;
                Low = Open;
                Close = Open;
            }

            var valueBigInt = value.ToBigInteger();

            if (valueBigInt > High.ToBigInteger())
            {
                High = value;
            }
            else if (valueBigInt < Low.ToBigInteger())
            {
                Low = value;
            }

            Close = value;
        }
    }
}