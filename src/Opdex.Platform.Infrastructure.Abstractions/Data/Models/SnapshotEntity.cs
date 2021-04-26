using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public abstract class SnapshotEntity
    {
        public short SnapshotType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}