using System;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;

namespace Opdex.Platform.Domain.Models.Pools.Snapshot
{
    public class LiquidityPoolSnapshot
    {
        public LiquidityPoolSnapshot(long liquidityPoolId, SnapshotType snapshotType, DateTime blockTime)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            if (snapshotType == SnapshotType.Unknown)
            {
                throw new ArgumentNullException(nameof(snapshotType));
            }

            LiquidityPoolId = liquidityPoolId;
            Reserves = new ReservesSnapshot();
            Rewards = new RewardsSnapshot();
            Staking = new StakingSnapshot();
            Volume = new VolumeSnapshot();
            Cost = new CostSnapshot();
            SnapshotType = snapshotType;
            StartDate = blockTime.ToStartOf(snapshotType);
            EndDate = blockTime.ToEndOf(snapshotType);;
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
        public ReservesSnapshot Reserves { get; }
        public RewardsSnapshot Rewards { get; private set; }
        public StakingSnapshot Staking { get; }
        public VolumeSnapshot Volume { get; private set; }
        public CostSnapshot Cost { get; }
        public SnapshotType SnapshotType { get; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime ModifiedDate { get; }

        public void ResetStaleSnapshot(decimal crsUsd, decimal stakingTokenUsd, DateTime blockTime)
        {
            if (Id == 0) return;

            // Reset Id for new Insert
            Id = 0;

            // Zero out Volume
            Volume = new VolumeSnapshot();

            // Zero Rewards
            Rewards = new RewardsSnapshot();

            // Refresh Staking
            Staking.RefreshStaking(stakingTokenUsd);

            // Cost should remain unchanged, token reserves unchanged
            // Refresh reserves (USD amounts)
            Reserves.RefreshReserves(crsUsd);

            TransactionCount = 0;

            StartDate = blockTime.ToStartOf(SnapshotType);
            EndDate = blockTime.ToEndOf(SnapshotType);
        }

        public void ProcessSwapLog(SwapLog log, decimal crsUsd, bool isStakingPool, uint transactionFee, bool marketFeeEnabled)
        {
            Volume.SetVolume(log, crsUsd);
            Rewards.SetRewards(Volume.VolumeUsd, Staking.StakingWeight, isStakingPool, transactionFee, marketFeeEnabled);
        }

        public void ProcessReservesLog(ReservesLog log, decimal crsUsd, ulong srcSats)
        {
            Reserves.SetReserves(log, crsUsd);
            Cost.SetCost(log, srcSats);
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