using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using System;

namespace Opdex.Platform.Domain.Models.LiquidityPools;

public class LiquidityPoolSummary : BlockAudit
{
    public LiquidityPoolSummary(ulong liquidityPoolId, ulong createdBlock) : base(createdBlock, createdBlock)
    {
        if (liquidityPoolId < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(liquidityPoolId), "LiquidityPoolId must be greater than 0.");
        }

        LiquidityPoolId = liquidityPoolId;
    }

    public LiquidityPoolSummary(ulong id, ulong liquidityPoolId, decimal liquidity, decimal dailyLiquidityUsdChangePercent, decimal volume,
                                ulong stakingWeight, decimal dailyStakingWeightChangePercent, ulong lockedCrs, UInt256 lockedSrc,
                                ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        LiquidityPoolId = liquidityPoolId;
        LiquidityUsd = liquidity;
        DailyLiquidityUsdChangePercent = dailyLiquidityUsdChangePercent;
        VolumeUsd = volume;
        StakingWeight = stakingWeight;
        DailyStakingWeightChangePercent = dailyStakingWeightChangePercent;
        LockedCrs = lockedCrs;
        LockedSrc = lockedSrc;
    }

    public ulong Id { get; }
    public ulong LiquidityPoolId { get; }
    public decimal LiquidityUsd { get; private set; }
    public decimal DailyLiquidityUsdChangePercent { get; private set; }
    public decimal VolumeUsd { get; private set; }
    public ulong StakingWeight { get; private set; }
    public decimal DailyStakingWeightChangePercent { get; private set; }
    public ulong LockedCrs { get; private set; }
    public UInt256 LockedSrc { get; private set; }

    public void Update(LiquidityPoolSnapshot snapshot, ulong blockHeight)
    {
        LiquidityUsd = snapshot.Reserves.Usd.Close;
        VolumeUsd = snapshot.Volume.Usd;
        StakingWeight = (ulong)snapshot.Staking.Weight.Close;
        LockedCrs = snapshot.Reserves.Crs.Close;
        LockedSrc = snapshot.Reserves.Src.Close;
        DailyLiquidityUsdChangePercent = MathExtensions.PercentChange(snapshot.Reserves.Usd.Close,
                                                                      snapshot.Reserves.Usd.Open);
        DailyStakingWeightChangePercent = MathExtensions.PercentChange(snapshot.Staking.Weight.Close,
                                                                       snapshot.Staking.Weight.Open,
                                                                       TokenConstants.Opdex.Sats);
        SetModifiedBlock(blockHeight);
    }
}