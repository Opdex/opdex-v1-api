using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    /// <summary>
    /// Staking event.
    /// </summary>
    public abstract class StakeEvent : TransactionEvent
    {
        /// <summary>
        /// Address of the staker.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Staker { get; set; }

        /// <summary>
        /// Amount of staking tokens transacted.
        /// </summary>
        /// <example>"800.00000000"</example>
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// Updated balance of the staker.
        /// </summary>
        /// <example>"1000.00000000"</example>
        public FixedDecimal StakerBalance { get; set; }

        /// <summary>
        /// Total staking tokens.
        /// </summary>
        /// <example>"750000000.00000000"</example>
        public FixedDecimal TotalStaked { get; set; }
    }
}
