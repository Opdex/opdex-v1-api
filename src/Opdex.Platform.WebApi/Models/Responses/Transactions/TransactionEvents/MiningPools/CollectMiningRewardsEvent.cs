using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    /// <summary>
    /// Collect mining rewards event.
    /// </summary>
    public class CollectMiningRewardsEvent : TransactionEvent
    {
        /// <summary>
        /// Address of the miner.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Miner { get; set; }

        /// <summary>
        /// Governance token reward amount.
        /// </summary>
        /// <example>200.00000000</example>
        public FixedDecimal Amount { get; set; }
    }
}
