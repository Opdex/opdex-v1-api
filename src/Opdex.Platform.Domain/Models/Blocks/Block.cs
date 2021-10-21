using System;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.Blocks
{
    public class Block
    {
        public Block(ulong height, Sha256 hash, DateTime time, DateTime medianTime)
        {
            if (height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Block height must be greater than 0.");
            }

            if (time.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(time), "Time must be set.");
            }

            if (medianTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(medianTime), "Median time must be set.");
            }

            Height = height;
            Hash = hash;
            Time = time;
            MedianTime = medianTime;
        }

        public ulong Height { get; }
        public Sha256 Hash { get; }
        public DateTime Time { get; }
        public DateTime MedianTime { get; }
    }
}
