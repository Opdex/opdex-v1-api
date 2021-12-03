using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;

public class LiquidityPoolSnapshot
{
    public LiquidityPoolSnapshot(ulong liquidityPoolId, SnapshotType snapshotType, DateTime blockTime)
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

    public LiquidityPoolSnapshot(ulong id, ulong liquidityPoolId, long transactionCount, ReservesSnapshot reserves, RewardsSnapshot rewards,
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

    public ulong Id { get; private set; }
    public ulong LiquidityPoolId { get; }
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

    /// <summary>
    /// Determine if the snapshot is stale compared to a provided datetime.
    /// </summary>
    /// <param name="endDate">The current end date to check against</param>
    /// <returns>true if the record is stale</returns>
    public bool IsStale(DateTime endDate) => EndDate < endDate;

    /// <summary>
    /// Reset a stale snapshot to a new instance with a 0 Id and update values that carryover
    /// such as staking totals, reserves, token costs etc.
    /// </summary>
    /// <param name="crsUsd">The USD price of a single CRS token.</param>
    /// <param name="stakingTokenUsd">The USD cost of a single staking token in the pool.</param>
    /// <param name="srcSats">The total sats per single full SRC token in the pool.</param>
    /// <param name="blockTime">The block time that represents this new snapshot.</param>
    public void ResetStaleSnapshot(decimal crsUsd, decimal stakingTokenUsd, ulong srcSats, DateTime blockTime)
    {
        // Reset Id for new Insert
        Id = 0;

        // Zero out Volume
        Volume = new VolumeSnapshot();

        // Zero Rewards
        Rewards = new RewardsSnapshot();

        // Refresh Staking
        Staking.Refresh(stakingTokenUsd);

        // Refresh costs (mainly reset OHLC)
        Cost.Refresh(Reserves.Crs.Close, Reserves.Src.Close, srcSats);

        // Refresh reserves (USD amounts)
        Reserves.Refresh(crsUsd);

        TransactionCount = 0;

        StartDate = blockTime.ToStartOf(SnapshotType);
        EndDate = blockTime.ToEndOf(SnapshotType);
    }

    /// <summary>
    /// Rewinds a daily snapshot using all existing hourly snapshots from the same day.
    /// </summary>
    /// <param name="snapshots">List of all hourly snapshots for the day.</param>
    public void RewindDailySnapshot(IList<LiquidityPoolSnapshot> snapshots)
    {
        if (!snapshots.Any())
        {
            return;
        }

        // This snapshot must be a day
        if (SnapshotType != SnapshotType.Daily)
        {
            throw new Exception("Only daily snapshots can be rewound.");
        }

        // All provided snapshots must be valid
        var allValidSnapshots = snapshots.All(s =>
        {
            var matchingLiquidityPoolId = s.LiquidityPoolId == LiquidityPoolId;
            var isHourlyType = s.SnapshotType == SnapshotType.Hourly;
            var sameDay = s.StartDate.Date == StartDate.Date && s.EndDate.Date == EndDate.Date;

            return isHourlyType && sameDay && matchingLiquidityPoolId;
        });

        if (!allValidSnapshots)
        {
            throw new Exception("Daily snapshots can only rewind using hourly snapshots.");
        }

        // Verify order is correct
        snapshots = snapshots.OrderBy(snapshot => snapshot.EndDate).ToList();

        // Volume will add all hourly volume totals
        Volume = new VolumeSnapshot(snapshots.Select(snapshot => snapshot.Volume).ToList());

        // Rewards add all hourly reward tokens
        Rewards = new RewardsSnapshot(snapshots.Select(snapshot => snapshot.Rewards).ToList());

        // Staking takes the latest total
        Staking = new StakingSnapshot(snapshots.Select(snapshot => snapshot.Staking).ToList());

        // Reserves take the latest total
        Reserves = new ReservesSnapshot(snapshots.Select(snapshot => snapshot.Reserves).ToList());

        // Cost is rebuilt for OHLC using all cost snapshots for the day
        Cost = new CostSnapshot(snapshots.Select(snapshot => snapshot.Cost).ToList());

        // Transaction counts add the total of each hour
        TransactionCount = snapshots.Sum(snapshot => snapshot.TransactionCount);
    }

    /// <summary>
    /// Takes current USD token prices and using updates the USD pricing of staking and pool reserves accordingly.
    /// </summary>
    /// <param name="crsUsd">The USD cost of a single CRS token.</param>
    /// <param name="stakingTokenUsd">The USD cost of a single staking token in the pool.</param>
    public void RefreshSnapshotFiatAmounts(decimal crsUsd, decimal stakingTokenUsd)
    {
        // Refresh staking USD amounts
        Staking.Update(stakingTokenUsd);

        // Refresh reserve USD amounts
        Reserves.Update(crsUsd);
    }

    /// <summary>
    /// Process a swap log by updating the volume and rewards amounts.
    /// </summary>
    /// <param name="log">The swap log to process.</param>
    /// <param name="crsUsd">The USD cost of a single CRS token.</param>
    /// <param name="srcUsd">The USD cost of a single SRC token in the pool.</param>
    /// <param name="srcSats">The total sats per single full SRC token in the pool.</param>
    /// <param name="isStakingPool">Flag indicating if its a staking pool or not, used to determine provider vs staker rewards.</param>
    /// <param name="transactionFee">The transaction fee as uint the paid in the swap (0-10) - staking market is 3.</param>
    /// <param name="marketFeeEnabled">Flat indicating if there is a market fee to the market owners, used to determine rewards.</param>
    public void ProcessSwapLog(SwapLog log, decimal crsUsd, decimal srcUsd, ulong srcSats, bool isStakingPool, uint transactionFee, bool marketFeeEnabled)
    {
        Volume.Update(log, crsUsd, srcUsd, srcSats);
        Rewards.UpdatePoolRewards(Volume.Usd, Staking.Weight.Close, isStakingPool, transactionFee, marketFeeEnabled);
    }

    /// <summary>
    /// Process a reserves log by updating reserve totals and token costs.
    /// </summary>
    /// <param name="log">The reserves log to process.</param>
    /// <param name="crsUsd">The USD cost of a single CRS token.</param>
    /// <param name="srcSats">The total sats per single full SRC token in the pool.</param>
    public void ProcessReservesLog(ReservesLog log, decimal crsUsd, ulong srcSats)
    {
        Reserves.Update(log, crsUsd);
        Cost.Update(log.ReserveCrs, log.ReserveSrc, srcSats);
    }

    /// <summary>
    /// Process a staking log by updating the staking totals
    /// </summary>
    /// <param name="log">The stake log to be processed.</param>
    /// <param name="stakingTokenUsd">The USD amount per full staking token.</param>
    public void ProcessStakingLog(StakeLog log, decimal stakingTokenUsd)
    {
        Staking.Update(log, stakingTokenUsd);
    }

    /// <summary>
    /// Increments the transaction count by 1.
    /// </summary>
    public void IncrementTransactionCount()
    {
        TransactionCount += 1;
    }
}