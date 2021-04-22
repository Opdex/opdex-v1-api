using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public class BlockEntity : AuditEntity
    {
        public ulong Height { get; set; }
        public string Hash { get; set; }
        public DateTime Time { get; set; }
        public DateTime MedianTime { get; set; }
    }
}