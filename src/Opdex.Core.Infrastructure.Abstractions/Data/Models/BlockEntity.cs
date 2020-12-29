using System;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Models
{
    public class BlockEntity
    {
        public ulong Height { get; set; }
        public string Hash { get; set; }
        public DateTime Time { get; set; }
        public DateTime MedianTime { get; set; }
    }
}