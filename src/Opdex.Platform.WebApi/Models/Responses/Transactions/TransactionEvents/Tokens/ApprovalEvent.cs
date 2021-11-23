using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens
{
    /// <summary>
    /// Approval event.
    /// </summary>
    public class ApprovalEvent : TransactionEvent
    {
        /// <summary>
        /// Address of the SRC token owner.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Owner { get; set; }

        /// <summary>
        /// Address approved to spend the tokens.
        /// </summary>
        /// <example>t8XpH1pNYDgCnqk91ZQKLgpUVeJ7XmomLT</example>
        public Address Spender { get; set; }

        /// <summary>
        /// Amount of SRC tokens.
        /// </summary>
        /// <example>1000.00000000</example>
        public FixedDecimal Amount { get; set; }
    }
}
