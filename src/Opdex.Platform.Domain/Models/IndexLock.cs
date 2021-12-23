using System;

namespace Opdex.Platform.Domain.Models;

public class IndexLock
{
    public IndexLock(bool available, bool locked, string instanceId, IndexLockReason reason, DateTime modifiedDate)
    {
        Available = available;
        Locked = locked;
        InstanceId = instanceId;
        Reason = reason;
        ModifiedDate = modifiedDate;
    }

    public bool Available { get; }
    public bool Locked { get; }
    public string InstanceId { get; }
    public IndexLockReason Reason { get; }
    public DateTime ModifiedDate { get; }
}

public enum IndexLockReason
{
    Deploying = 1, Indexing = 2, Searching = 3, Rewinding = 4, Resyncing = 5
}
