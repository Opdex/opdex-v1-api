using System;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Domain.Models
{
    public class Block
    {
        public Block(ulong height, string hash, DateTime time, DateTime medianTime)
        {
            Height = height > 0 ? height : throw new ArgumentOutOfRangeException(nameof(height));
            Hash = hash.HasValue() ? hash : throw new ArgumentNullException(nameof(hash));
            Time = time.Equals(default) ? throw new ArgumentOutOfRangeException(nameof(time)) : time;
            MedianTime = medianTime.Equals(default) ? throw new ArgumentOutOfRangeException(nameof(medianTime)) : medianTime;
        }
        
        public ulong Height { get; }
        public string Hash { get; }
        public DateTime Time { get; }
        public DateTime MedianTime { get; }
    }
}