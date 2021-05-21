using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public class IndexLockEntity
    {
        public bool Locked { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}