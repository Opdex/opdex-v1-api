using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Governances
{
    /// <summary>
    /// Liquidity pool nomination event.
    /// </summary>
    public class NominationEventResponseModel : TransactionEventResponseModel
    {
        /// <summary>
        /// Address of the staking pool.
        /// </summary>
        /// <example>tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L</example>
        public Address StakingPool { get; set; }

        /// <summary>
        /// Address of the mining pool.
        /// </summary>
        /// <example>tNgQhNxvachxKGvRonk2S8nrpYi44carYv</example>
        public Address MiningPool { get; set; }

        /// <summary>
        /// Goverenance token weight of the nominated pool.
        /// </summary>
        /// <example>100000.00000000</example>
        public FixedDecimal Weight { get; set; }
    }
}
