using System;

namespace Opdex.Platform.Domain.Models
{
    public class IndexLock
    {
        public IndexLock(bool available, bool locked, DateTime modifiedDate)
        {
            Available = available;
            Locked = locked;
            ModifiedDate = modifiedDate;
        }

        public bool Available { get; }
        public bool Locked { get; }
        public DateTime ModifiedDate { get; }
    }
}
