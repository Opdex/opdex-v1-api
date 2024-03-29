using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents;

/// <summary>
/// Event that occurred in a transaction.
/// </summary>
public abstract class TransactionEvent
{
    /// <summary>
    /// Event type identifier.
    /// </summary>
    /// <example>SwapEvent</example>
    public TransactionEventType EventType { get; set; }

    /// <summary>
    /// Address of the contract that logged the event.
    /// </summary>
    /// <example>tFCyMPURX9pVWXuXQqYY2hJmRcCrhmBPfY</example>
    public Address Contract { get; set; }

    /// <summary>
    /// Position in the sequence the event occurred.
    /// </summary>
    /// <example>1</example>
    public int SortOrder { get; set; }
}
