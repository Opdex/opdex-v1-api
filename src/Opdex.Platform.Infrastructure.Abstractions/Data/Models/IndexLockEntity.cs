using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public class IndexLockEntity
    {
        public bool Available { get; set; }
        public bool Locked { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string InstanceId { get; set; }
    }
}
