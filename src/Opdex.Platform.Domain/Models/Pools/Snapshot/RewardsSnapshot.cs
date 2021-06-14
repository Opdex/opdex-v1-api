using System;

namespace Opdex.Platform.Domain.Models.Pools.Snapshot
{
    public class RewardsSnapshot
    {
        public RewardsSnapshot()
        {
            ProviderUsd = 0.00m;
            MarketUsd = 0.00m;
        }

        public RewardsSnapshot(decimal providerUsd, decimal marketUsd)
        {
            ProviderUsd = providerUsd;
            MarketUsd = marketUsd;
        }

        public decimal ProviderUsd { get; private set; }
        public decimal MarketUsd { get; private set; }

        public void SetRewards(decimal volumeUsd, string stakingWeight, bool isStakingPool, uint transactionFee, bool marketFeeEnabled)
        {
            var fee = transactionFee / (decimal)1000;
            var totalRewards = Math.Round(volumeUsd * fee, 2, MidpointRounding.AwayFromZero);

            // Zero staking weight, all fees to providers
            var emptyStakingPool = isStakingPool && stakingWeight.Equals("0");

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