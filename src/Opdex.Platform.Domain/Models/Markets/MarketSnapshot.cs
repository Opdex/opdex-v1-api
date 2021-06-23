using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Pools.Snapshots;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class MarketSnapshot
    {
        public MarketSnapshot(long marketId, SnapshotType snapshotType, DateTime dateTime)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than 0.");
            }

            MarketId = marketId;
            Liquidity = 0.00m;
            Volume = 0.00m;
            Staking = new StakingSnapshot();
            Rewards = new RewardsSnapshot();
            SnapshotType = snapshotType;
            StartDate = dateTime.ToStartOf(snapshotType);
            EndDate = dateTime.ToEndOf(snapshotType);
        }

        public MarketSnapshot(long id, long marketId, decimal liquidity, decimal volume, StakingSnapshot staking, RewardsSnapshot rewards,
                              SnapshotType snapshotType, DateTime startDate, DateTime endDate)
        {
            Id = id;
            MarketId = marketId;
            Liquidity = liquidity;
            Volume = volume;
            Staking = staking;
            Rewards = rewards;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }

        public long Id { get; private set; }
        public long MarketId { get; }
        public decimal Liquidity { get; private set; }
        public decimal Volume { get; private set; }
        public StakingSnapshot Staking { get; private set; }
        public RewardsSnapshot Rewards { get; private set; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public void ResetStaleSnapshot(DateTime blockTime)
        {
            Id = 0;
            StartDate = blockTime.ToStartOf(SnapshotType);
            EndDate = blockTime.ToEndOf(SnapshotType);
            ResetCurrentSnapshot();
        }

        public void ResetCurrentSnapshot()
        {
            Liquidity = 0;
            Volume = 0;
            Staking = new StakingSnapshot();
            Rewards = new RewardsSnapshot();
        }

        public void Update(LiquidityPoolSnapshot lpSnapshot)
        {
            Liquidity += lpSnapshot.Reserves.Usd;
            Volume += lpSnapshot.Volume.Usd;

            var stakingUsd = Staking.Usd + lpSnapshot.Staking.Usd;
            var stakingWeight = Staking.Weight.ToBigInteger() + lpSnapshot.Staking.Weight.ToBigInteger();

            Staking = new StakingSnapshot(stakingWeight.ToString(), stakingUsd);

            var rewardsMarketUsd = Rewards.MarketUsd + lpSnapshot.Rewards.MarketUsd;
            var rewardsProviderUsd = Rewards.ProviderUsd + lpSnapshot.Rewards.ProviderUsd;

            Rewards = new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd);
        }
    }
}
