using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents
{
    /// <summary>
    /// Ownership update event.
    /// </summary>
    public class OwnershipEvent : TransactionEvent
    {
        /// <summary>
        /// Previous address.
        /// </summary>
        /// <example>tHYHem7cLKgoLkeb792yn4WayqKzLrjJak</example>
        public Address From { get; set; }

        /// <summary>
        /// Updated address.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address To { get; set; }
    }
}
