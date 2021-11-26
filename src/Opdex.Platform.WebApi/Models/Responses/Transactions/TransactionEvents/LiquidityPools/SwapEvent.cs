using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    /// <summary>
    /// Swap event.
    /// </summary>
    public class SwapEvent : TransactionEvent
    {
        /// <summary>
        /// Address of the sender.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Sender { get; set; }

        /// <summary>
        /// Address that the output tokens are sent to.
        /// </summary>
        /// <example>tHYHem7cLKgoLkeb792yn4WayqKzLrjJak</example>
        public Address To { get; set; }

        /// <summary>
        /// Amount of CRS tokens input.
        /// </summary>
        /// <example>"0.00000000"</example>
        public FixedDecimal AmountCrsIn { get; set; }

        /// <summary>
        /// Amount of SRC tokens input.
        /// </summary>
        /// <example>"0.000500000000000000"</example>
        public FixedDecimal AmountSrcIn { get; set; }

        /// <summary>
        /// Amount of CRS tokens output.
        /// </summary>
        /// <example>"2.50000000"</example>
        public FixedDecimal AmountCrsOut { get; set; }

        /// <summary>
        /// Amount of SRC tokens output.
        /// </summary>
        /// <example>"0.000000000000000000"</example>
        public FixedDecimal AmountSrcOut { get; set; }

        /// <summary>
        /// Address of the SRC token.
        /// </summary>
        /// <example>tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3</example>
        public Address SrcToken { get; set; }
    }
}
