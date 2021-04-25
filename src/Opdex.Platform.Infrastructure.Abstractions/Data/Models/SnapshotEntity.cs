using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public abstract class SnapshotEntity
    {
        public short SnapshotType { get; set; }
        public DateTime SnapshotStartDate { get; set; }
        public DateTime SnapshotEndDate { get; set; }
    }
}