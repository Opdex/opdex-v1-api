using System;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Domain.Models
{
    public class Block
    {
        public Block(ulong height, string hash, DateTime time, DateTime medianTime)
        {
            if (height < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (!hash.HasValue())
            {
                throw new ArgumentNullException(nameof(hash));
            }

            if (time.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(time));
            }
            
            if (medianTime.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(medianTime));
            }

            Height = height;
            Hash = hash;
            Time = time;
            MedianTime = medianTime;
        }
        
        public ulong Height { get; }
        public string Hash { get; }
        public DateTime Time { get; }
        public DateTime MedianTime { get; }
    }
}