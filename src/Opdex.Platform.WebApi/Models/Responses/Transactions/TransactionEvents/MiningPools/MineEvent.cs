using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    /// <summary>
    /// Mining event.
    /// </summary>
    public abstract class MineEvent : TransactionEvent
    {
        /// <summary>
        /// Address of the miner.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        public Address Miner { get; set; }

        /// <summary>
        /// Amount of tokens mined.
        /// </summary>
        /// <example>"500000.00000000"</example>
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// Total supply of mining tokens.
        /// </summary>
        /// <example>"1000000000.00000000"</example>
        public FixedDecimal TotalSupply { get; set; }

        /// <summary>
        /// Mining token balance of the miner.
        /// </summary>
        /// <example>"2500000.00000000"</example>
        public FixedDecimal MinerBalance { get; set; }
    }
}
