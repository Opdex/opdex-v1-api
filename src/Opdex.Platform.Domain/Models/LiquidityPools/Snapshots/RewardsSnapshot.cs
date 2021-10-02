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

        internal void SetRewards(decimal volumeUsd, UInt256 stakingWeight, bool isStakingPool, uint transactionFee, bool marketFeeEnabled)
        {
            var fee = transactionFee / (decimal)1000;
            var totalRewards = Math.Round(volumeUsd * fee, 2, MidpointRounding.AwayFromZero);

            // Zero staking weight, all fees to providers
            var emptyStakingPool = isStakingPool && stakingWeight == UInt256.Zero;

            if (emptyStakingPool || !marketFeeEnabled)
            {
                ProviderUsd = totalRewards;
            }
            else // Split rewards
            {
                MarketUsd = Math.Round(totalRewards / 6, 2, MidpointRounding.AwayFromZero); // 1/6
                ProviderUsd = totalRewards - MarketUsd; // 5/6
            }
        }
    }
}
