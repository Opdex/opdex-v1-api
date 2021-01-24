using System;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Models
{
    public class BlockEntity
    {
        public ulong Height { get; set; }
        public string Hash { get; set; }
        public long Time { get; set; }
        public long MedianTime { get; set; }
    }
}