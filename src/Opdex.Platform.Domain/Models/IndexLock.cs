using System;

namespace Opdex.Platform.Domain.Models
{
    public class IndexLock
    {
        public IndexLock(bool available, bool locked, string instanceId, DateTime modifiedDate)
        {
            Available = available;
            Locked = locked;
            ModifiedDate = modifiedDate;
            InstanceId = instanceId;
        }

        public bool Available { get; }
        public bool Locked { get; }
        public DateTime ModifiedDate { get; }
        public string InstanceId { get; }
    }
}
