using System;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.Markets
{
    public class MarketSnapshot
    {
        public MarketSnapshot(ulong marketId, SnapshotType snapshotType, DateTime dateTime)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater than 0.");
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType), "Snapshot type must be valid.");
            }

            MarketId = marketId;
            LiquidityUsd = new Ohlc<decimal>();
            VolumeUsd = 0.00m;
            Staking = new StakingSnapshot();
            Rewards = new RewardsSnapshot();
            SnapshotType = snapshotType;
            StartDate = dateTime.ToStartOf(snapshotType);
            EndDate = dateTime.ToEndOf(snapshotType);
        }

        public MarketSnapshot(ulong id, ulong marketId, Ohlc<decimal> liquidity, decimal volume, StakingSnapshot staking, RewardsSnapshot rewards,
                              SnapshotType snapshotType, DateTime startDate, DateTime endDate)
        {
            Id = id;
            MarketId = marketId;
            LiquidityUsd = liquidity;
            VolumeUsd = volume;
            Staking = staking;
            Rewards = rewards;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
        }

        public ulong Id { get; private set; }
        public ulong MarketId { get; }
        public Ohlc<decimal> LiquidityUsd { get; }
        public decimal VolumeUsd { get; private set; }
        public StakingSnapshot Staking { get; }
        public RewardsSnapshot Rewards { get; private set; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Resets a stale snapshot back to an entirely new instance with a new identity.
        /// </summary>
        /// <param name="blockTime">The block time to be applied to the updated time frame.</param>
        /// <param name="stakingTokenUsd">The USD price of the staking token at the time of the reset.</param>
        public void ResetStaleSnapshot(DateTime blockTime, decimal stakingTokenUsd)
        {
            Id = 0;
            StartDate = blockTime.ToStartOf(SnapshotType);
            EndDate = blockTime.ToEndOf(SnapshotType);
            VolumeUsd = 0;
            LiquidityUsd.Refresh(LiquidityUsd.Close);
            Staking.Refresh(stakingTokenUsd);
            Rewards = new RewardsSnapshot();
        }

        /// <summary>
        /// Add to the current snapshot state, updating liquidity, volume, staking and reward amounts.
        /// </summary>
        /// <param name="snapshots">A list of liquidity pool snapshots to get totals of.</param>
        public void Update(IList<LiquidityPoolSnapshot> snapshots)
        {
            // All provided snapshots must be valid
            var allValidSnapshots = snapshots.All(s =>
            {
                var isDailyType = s.SnapshotType == SnapshotType.Daily;
                var sameDay = s.StartDate.Date == StartDate.Date && s.EndDate.Date == EndDate.Date;
                var uniquePool = snapshots.Count(p => p.LiquidityPoolId == s.LiquidityPoolId) == 1;

                return isDailyType && sameDay && uniquePool;
            });

            if (!allValidSnapshots)
            {
                throw new Exception("Market snapshots can only use same day liquidity pool daily snapshot types.");
            }

            var liquidityUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Reserves.Usd.Close);
            var volumeUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Volume.Usd);
            var stakingWeight = snapshots.Aggregate(UInt256.Zero, (a,c) => a + c.Staking.Weight.Close);
            var stakingUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Staking.Usd.Close);
            var rewardsMarketUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Rewards.MarketUsd);
            var rewardsProviderUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Rewards.ProviderUsd);

            LiquidityUsd.Update(liquidityUsd);
            Staking.Usd.Update(stakingUsd);
            Staking.Weight.Update(stakingWeight);
            VolumeUsd = volumeUsd;
            Rewards = new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd);
        }
    }
}
