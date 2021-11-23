using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools
{
    /// <summary>
    /// Enable mining event.
    /// </summary>
    public class EnableMiningEventResponseModel : TransactionEventResponseModel
    {
        /// <summary>
        /// Amount of tokens rewarded to the mining pool for mining.
        /// </summary>
        /// <example>500000.00000000</example>
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// Amount of tokens mined per block.
        /// </summary>
        /// <example>100.00000000</example>
        public FixedDecimal RewardRate { get; set; }

        /// <summary>
        /// Block number that the mining period ends.
        /// </summary>
        /// <example>690000</example>
        public ulong MiningPeriodEndBlock { get; set; }
    }
}
