using System;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
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

            // Reset this snapshot
            ResetCurrentSnapshot();

            // Rebuild the snapshot
            foreach (var snapshot in snapshots)
            {
                Update(snapshot);
            }
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
        /// Resets the current snapshot back to new but maintains is Id.
        /// Used when daily market snapshots are updated.
        /// </summary>
        public void ResetCurrentSnapshot()
        {
            LiquidityUsd = new Ohlc<decimal>();
            VolumeUsd = 0;
            Staking = new StakingSnapshot();
            Rewards = new RewardsSnapshot();
        }

        /// <summary>
        /// Add to the current snapshot state, updating liquidity, volume, staking and reward amounts.
        /// </summary>
        /// <param name="snapshot">The liquidity pool snapshot to add.</param>
        public void Update(LiquidityPoolSnapshot snapshot)
        {
            if (snapshot.SnapshotType != SnapshotType.Daily)
            {
                throw new Exception("Only daily liquidity snapshots can be used.");
            }

            if (snapshot.StartDate.Date != StartDate.Date || snapshot.EndDate.Date != EndDate.Date)
            {
                throw new Exception("Market and liquidity pool snapshot dates do not match.");
            }

            LiquidityUsd.Update(LiquidityUsd.Close + snapshot.Reserves.Usd.Close);
            VolumeUsd += snapshot.Volume.Usd;

            var stakingUsd = Staking.Usd.Close + snapshot.Staking.Usd.Close;
            var stakingWeight = Staking.Weight.Close + snapshot.Staking.Weight.Close;

            Staking.Usd.Update(stakingUsd);
            Staking.Weight.Update(stakingWeight);

            var rewardsMarketUsd = Rewards.MarketUsd + snapshot.Rewards.MarketUsd;
            var rewardsProviderUsd = Rewards.ProviderUsd + snapshot.Rewards.ProviderUsd;

            Rewards = new RewardsSnapshot(rewardsProviderUsd, rewardsMarketUsd);
        }
    }
}
