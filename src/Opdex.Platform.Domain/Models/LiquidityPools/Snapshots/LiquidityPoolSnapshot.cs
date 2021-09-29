using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots
{
    public class LiquidityPoolSnapshot
    {
        public LiquidityPoolSnapshot(long liquidityPoolId, SnapshotType snapshotType, DateTime blockTime)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), $"{nameof(liquidityPoolId)} must be greater than 0.");
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType), $"{nameof(snapshotType)} must be a valid type.");
            }

            LiquidityPoolId = liquidityPoolId;
            Reserves = new ReservesSnapshot();
            Rewards = new RewardsSnapshot();
            Staking = new StakingSnapshot();
            Volume = new VolumeSnapshot();
            Cost = new CostSnapshot();
            SnapshotType = snapshotType;
            StartDate = blockTime.ToStartOf(snapshotType);
            EndDate = blockTime.ToEndOf(snapshotType);
        }

        public LiquidityPoolSnapshot(long id, long liquidityPoolId, long transactionCount, ReservesSnapshot reserves, RewardsSnapshot rewards,
            StakingSnapshot staking, VolumeSnapshot volume, CostSnapshot cost, SnapshotType snapshotType, DateTime startDate, DateTime endDate, DateTime modifiedDate)
        {
            Id = id;
            LiquidityPoolId = liquidityPoolId;
            TransactionCount = transactionCount;
            Reserves = reserves;
            Rewards = rewards;
            Staking = staking;
            Volume = volume;
            Cost = cost;
            SnapshotType = snapshotType;
            StartDate = startDate;
            EndDate = endDate;
            ModifiedDate = modifiedDate;
        }

        public long Id { get; private set; }
        public long LiquidityPoolId { get; }
        public long TransactionCount { get; private set; }
        public ReservesSnapshot Reserves { get; private set; }
        public RewardsSnapshot Rewards { get; private set; }
        public StakingSnapshot Staking { get; private set; }
        public VolumeSnapshot Volume { get; private set; }
        public CostSnapshot Cost { get; private set; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime ModifiedDate { get; }

        public void ResetStaleSnapshot(decimal crsUsd, decimal srcUsd, decimal stakingTokenUsd, ulong srcSats, DateTime blockTime)
        {
            // Reset Id for new Insert
            Id = 0;

            // Zero out Volume
            Volume = new VolumeSnapshot();

            // Zero Rewards
            Rewards = new RewardsSnapshot();

            // Refresh Staking
            Staking.RefreshStaking(stakingTokenUsd);

            // Refresh costs (mainly reset OHLC)
            Cost.SetCost(Reserves.Crs, Reserves.Src, srcSats, true);

            // Refresh reserves (USD amounts)
            Reserves.RefreshReserves(crsUsd, srcUsd, srcSats);

            TransactionCount = 0;

            StartDate = blockTime.ToStartOf(SnapshotType);
            EndDate = blockTime.ToEndOf(SnapshotType);
        }

        /// <summary>
        /// Rewinds a snapshot by resetting everything then using existing, lower level snapshot to rebuild this instance.
        /// </summary>
        public void RewindSnapshot(IList<LiquidityPoolSnapshot> snapshots)
        {
            // Technically, we should be able to return out if none exist. Would mean that this
            // snapshot being refreshed _should_ be a new Zero instance. For now, refreshing everything.
            var exists = snapshots.Any();

            // Verify order is correct
            if (exists) snapshots = snapshots.OrderBy(snapshot => snapshot.EndDate).ToList();

            Volume = exists ? new VolumeSnapshot(snapshots.Select(snapshot => snapshot.Volume).ToList()) : new VolumeSnapshot();
            Rewards = exists ? new RewardsSnapshot(snapshots.Select(snapshot => snapshot.Rewards).ToList()) : new RewardsSnapshot();
            Staking = exists ? new StakingSnapshot(snapshots.Select(snapshot => snapshot.Staking).ToList()) : new StakingSnapshot();
            Reserves = exists ? new ReservesSnapshot(snapshots.Last().Reserves) : new ReservesSnapshot();
            Cost = exists ? new CostSnapshot(snapshots.Select(snapshot => snapshot.Cost).ToList()) : new CostSnapshot();
            TransactionCount = exists ? snapshots.Sum(snapshot => snapshot.TransactionCount) : 0;
        }

        public void RefreshSnapshot(decimal crsUsd, decimal srcUsd, decimal stakingTokenUsd, ulong srcSats)
        {
            // Refresh staking USD amounts
            Staking.RefreshStaking(stakingTokenUsd);

            // Refresh reserve USD amounts
            Reserves.RefreshReserves(crsUsd, srcUsd, srcSats);
        }

        public void ProcessSwapLog(SwapLog log, decimal crsUsd, decimal srcUsd, ulong srcSats, bool isStakingPool, uint transactionFee, bool marketFeeEnabled)
        {
            Volume.SetVolume(log, crsUsd, srcUsd, srcSats);
            Rewards.SetRewards(Volume.Usd, Staking.Weight, isStakingPool, transactionFee, marketFeeEnabled);
        }

        public void ProcessReservesLog(ReservesLog log, decimal crsUsd, decimal srcUsd, ulong srcSats)
        {
            Reserves.SetReserves(log, crsUsd, srcUsd, srcSats);
            Cost.SetCost(log.ReserveCrs, log.ReserveSrc, srcSats);
        }

        public void ProcessStakingLog(StakeLog log, decimal stakingTokenUsd)
        {
            Staking.SetStaking(log, stakingTokenUsd);
        }

        public void IncrementTransactionCount()
        {
            TransactionCount += 1;
        }
    }
}
