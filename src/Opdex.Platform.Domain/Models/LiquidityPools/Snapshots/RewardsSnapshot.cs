using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots
{
    public class RewardsSnapshot
    {
        public RewardsSnapshot()
        {
            ProviderUsd = 0.00000000m;
            MarketUsd = 0.00000000m;
        }

        public RewardsSnapshot(IList<RewardsSnapshot> snapshots)
        {
            ProviderUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.ProviderUsd);
            MarketUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.MarketUsd);
        }

        public RewardsSnapshot(decimal providerUsd, decimal marketUsd)
        {
            if (providerUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(providerUsd), $"{nameof(providerUsd)} must be greater or equal to 0.");
            }

            if (marketUsd < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(marketUsd), $"{nameof(marketUsd)} must be greater or equal to 0.");
            }

            ProviderUsd = providerUsd;
            MarketUsd = marketUsd;
        }

        public decimal ProviderUsd { get; private set; }
        public decimal MarketUsd { get; private set; }

        /// <summary>
        /// Update the rewards for liquidity pools using the current volume and staking properties to calculate provider vs market rewards.
        /// </summary>
        /// <param name="volumeUsd">The USD total volume for the snapshot period.</param>
        /// <param name="stakingWeight">The staking weight for the snapshot period, 0 for non staking pools.</param>
        /// <param name="isStakingPool">Boolean flag indicating if the pool has staking enabled.</param>
        /// <param name="transactionFee">The transaction fee per swap transaction.</param>
        /// <param name="marketFeeEnabled">
        /// Flag indicating if the market fee is enabled or not, will always be true for staking pools
        /// sometimes true for non staking pools depending on the market.
        /// </param>
        internal void UpdatePoolRewards(decimal volumeUsd, UInt256 stakingWeight, bool isStakingPool, uint transactionFee, bool marketFeeEnabled)
        {
            (decimal providerUsd, decimal marketUsd) = MathExtensions.VolumeBasedRewards(volumeUsd, stakingWeight, isStakingPool,
                                                                                         transactionFee, marketFeeEnabled);

            ProviderUsd = providerUsd;
            MarketUsd = marketUsd;
        }
    }
}
