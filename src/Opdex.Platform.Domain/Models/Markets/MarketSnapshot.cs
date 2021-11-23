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
        public Ohlc<decimal> LiquidityUsd { get; private set; }
        public decimal VolumeUsd { get; private set; }
        public StakingSnapshot Staking { get; private set; }
        public RewardsSnapshot Rewards { get; private set; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Rewinds a daily snapshot using all existing hourly snapshots from the same day.
        /// </summary>
        /// <param name="snapshots">List of all hourly snapshots for the day.</param>
        public void RewindSnapshot(IList<LiquidityPoolSnapshot> snapshots)
        {
            // This snapshot must be a day
            if (SnapshotType != SnapshotType.Daily)
            {
                throw new Exception("Only daily snapshots can be rewound.");
            }

            // All provided snapshots must be valid
            var allValidSnapshots = snapshots.All(s =>
            {
                var isDailyType = s.SnapshotType == SnapshotType.Daily;
                var sameDay = s.StartDate.Date == StartDate.Date && s.EndDate.Date == EndDate.Date;
                var uniquePool = snapshots.Count(p => p.LiquidityPoolId == s.Id) == 1;

                return isDailyType && sameDay && uniquePool;
            });

            if (!allValidSnapshots)
            {
                throw new Exception("Daily snapshots can only rewind using same day liquidity pool daily snapshot types.");
            }

            // Reset and update this snapshot
            ResetCurrentSnapshot();
            Update(snapshots);
        }

        /// <summary>
        /// Resets a stale snapshot back to an entirely new instance with a new identity.
        /// </summary>
        /// <param name="blockTime">The block time to be applied to the updated time frame.</param>
        public void ResetStaleSnapshot(DateTime blockTime)
        {
            Id = 0;
            StartDate = blockTime.ToStartOf(SnapshotType);
            EndDate = blockTime.ToEndOf(SnapshotType);
            ResetCurrentSnapshot();
        }

        /// <summary>
        /// Resets the current snapshot back to new but maintains its Id.
        /// Used when daily market snapshots are updated.
        /// </summary>
        public void ResetCurrentSnapshot()
        {
            VolumeUsd = 0;
            LiquidityUsd.Update(LiquidityUsd.Close, true);
            Staking.Refresh();
            Rewards = new RewardsSnapshot();
        }

        /// <summary>
        /// Add to the current snapshot state, updating liquidity, volume, staking and reward amounts.
        /// </summary>
        /// <param name="snapshots">A list of liquidity pool snapshots to get totals of.</param>
        public void Update(IList<LiquidityPoolSnapshot> snapshots)
        {
            if (snapshots.Any(snapshot => snapshot.SnapshotType != SnapshotType.Daily ||
                                          (snapshot.StartDate.Date != StartDate.Date || snapshot.EndDate.Date != EndDate.Date)))
            {
                throw new Exception("Only daily liquidity pool snapshots within the current market snapshot range can be used.");
            }

            var liquidityUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Reserves.Usd.Close);
            var volumeUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Volume.Usd);
            var stakingWeight = snapshots.Aggregate(UInt256.Zero, (a,c) => a + c.Staking.Weight.Close);
            var stakingUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Staking.Usd.Close);
            var rewardsMarketUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Rewards.MarketUsd);
            var rewardsProviderUsd = snapshots.Aggregate(0.00000000m, (a,c) => a + c.Rewards.ProviderUsd);

            LiquidityUsd.Update(liquidityUsd);
            VolumeUsd = volumeUsd;
            Staking.Usd.Update(stakingUsd);
            Staking.Weight.Update(stakingWeight);
            Rewards = new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd);
        }
    }
}
