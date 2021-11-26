using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens
{
    /// <summary>
    /// SRC token transfer event.
    /// </summary>
    public class TransferEvent : TransactionEvent
    {
        /// <summary>
        /// Address that the tokens were transferred from.
        /// </summary>
        /// <example>t8XpH1pNYDgCnqk91ZQKLgpUVeJ7XmomLT</example>
        public Address From { get; set; }

        /// <summary>
        /// Address of the recipient.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address To { get; set; }

        /// <summary>
        /// SRC token amount.
        /// </summary>
        /// <example>"500.00000000"</example>
        public FixedDecimal Amount { get; set; }
    }
}
