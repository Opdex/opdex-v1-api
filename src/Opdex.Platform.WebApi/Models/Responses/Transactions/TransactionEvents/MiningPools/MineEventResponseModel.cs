using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    /// <summary>
    /// Mining event.
    /// </summary>
    public abstract class MineEventResponseModel : TransactionEventResponseModel
    {
        /// <summary>
        /// Address of the miner.
        /// </summary>
        /// <example></example>
        public Address Miner { get; set; }

        /// <summary>
        /// Amount of tokens mined.
        /// </summary>
        /// <example></example>
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// Total supply of mining tokens.
        /// </summary>
        /// <example></example>
        public FixedDecimal TotalSupply { get; set; }

        /// <summary>
        /// Mining token balance of the miner.
        /// </summary>
        /// <example></example>
        public FixedDecimal MinerBalance { get; set; }
    }
}
