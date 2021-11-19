using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models
{
    public class Ohlc<T> where T : IComparable<T>
    {
        public Ohlc()
        {
            Open = default;
            High = default;
            Low = default;
            Close = default;
        }

        public Ohlc(T open, T high, T low, T close)
        {
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }

        public Ohlc(IList<Ohlc<T>> snapshots)
        {
            if (!snapshots.Any())
            {
                Update(default, true);
            }

            Open = snapshots.First().Open ?? default;
            High = snapshots.OrderByDescending(snapshot => snapshot.High).First().High ?? default;
            Low = snapshots.OrderBy(snapshot => snapshot.Low).First().Low ?? default;
            Close = snapshots.Last().Close ?? default;
        }

        public T Open { get; private set; }
        public T High { get; private set; }
        public T Low { get; private set; }
        public T Close { get; private set; }

        internal void Update(T value, bool reset = false)
        {
            if (Open.CompareTo(default) == 0 || reset)
            {
                Open = value;
                High = value;
                Low = value;
                Close = value;

                return;
            }

            if (value.CompareTo(High) > 0) High = value;
            else if (value.CompareTo(Low) < 0) Low = value;

            Close = value;
        }
    }
}
