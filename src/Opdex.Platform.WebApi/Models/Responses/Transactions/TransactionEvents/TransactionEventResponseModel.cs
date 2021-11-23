using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using System.Diagnostics.CodeAnalysis;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents
{
    /// <summary>
    /// Event that occurred in a transaction.
    /// </summary>
    public abstract class TransactionEventResponseModel
    {
        /// <summary>
        /// Event type identifier.
        /// </summary>
        /// <example>SwapEvent</example>
        public TransactionEventType EventType { get; set; }

        /// <summary>
        /// Address of the contract that logged the event.
        /// </summary>
        /// <example>tLrMcU1csbN7RxGjBMEnJeLoae3PxmQ9cr</example>
        [NotNull]
        public Address Contract { get; set; }

        /// <summary>
        /// Position in the sequence the event occurred.
        /// </summary>
        /// <example>1</example>
        public int SortOrder { get; set; }
    }
}
