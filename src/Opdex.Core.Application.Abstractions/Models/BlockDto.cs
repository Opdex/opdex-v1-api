using System;

namespace Opdex.Core.Application.Abstractions.Models
{
    public class BlockDto
    {
        public ulong Height { get; set; }
        public string Hash { get; set; }
        public DateTime Time { get; set; }
        public DateTime MedianTime { get; set; }
    }
}