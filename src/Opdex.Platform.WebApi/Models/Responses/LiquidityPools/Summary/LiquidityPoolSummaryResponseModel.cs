using NJsonSchema.Annotations;
using Opdex.Platform.WebApi.Models.Responses.MiningPools;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary
{
    public abstract class LiquidityPoolSummaryResponseModel
    {
        /// <summary>
        /// The liquidity pool's locked reserves.
        /// </summary>
        [NotNull]
        public ReservesResponseModel Reserves { get; set; }

        /// <summary>
        /// Rewards details based on transaction volume.
        /// </summary>
        [NotNull]
        public RewardsResponseModel Rewards { get; set; }

        /// <summary>
        /// Current volume based on the day.
        /// </summary>
        [NotNull]
        public VolumeResponseModel Volume { get; set; }

        /// <summary>
        /// The like for like cost of each token in the pool.
        /// </summary>
        [NotNull]
        public CostResponseModel Cost { get; set; }

        /// <summary>
        /// Staking details based on the pool's current status.
        /// </summary>
        public StakingResponseModel Staking { get; set; }

        /// <summary>
        /// The governance mining pool associated with the liquidity pool in a staking market.
        /// </summary>
        public MiningPoolResponseModel MiningPool { get; set; }
    }
}
