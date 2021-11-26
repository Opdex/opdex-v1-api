using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    /// <summary>
    /// Collect staking rewards event.
    /// </summary>
    public class CollectStakingRewardsEvent : TransactionEvent
    {
        /// <summary>
        /// Address of the staker.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Staker { get; set; }

        /// <summary>
        /// Staking reward.
        /// </summary>
        /// <example>"500.00000000"</example>
        public FixedDecimal Amount { get; set; }
    }
}
