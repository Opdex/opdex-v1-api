using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.Markets;

public class MarketSummary : BlockAudit
{
    public MarketSummary(ulong marketId, ulong createdBlock) : base(createdBlock, createdBlock)
    {
        MarketId = marketId > 0 ? marketId : throw new ArgumentOutOfRangeException(nameof(marketId), "MarketId must be greater than zero.");
    }

    public MarketSummary(ulong id, ulong marketId, decimal liquidityUsd, decimal dailyLiquidityUsdChangePercent, decimal volumeUsd, ulong stakingWeight,
                         decimal dailyStakingWeightChangePercent, decimal stakingUsd, decimal dailyStakingUsdChangePercent, decimal providerRewardsDailyUsd,
                         decimal marketRewardsDailyUsd, ulong liquidityPoolCount, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        MarketId = marketId;
        LiquidityUsd = liquidityUsd;
        DailyLiquidityUsdChangePercent = dailyLiquidityUsdChangePercent;
        VolumeUsd = volumeUsd;
        StakingWeight = stakingWeight;
        DailyStakingWeightChangePercent = dailyStakingWeightChangePercent;
        StakingUsd = stakingUsd;
        DailyStakingUsdChangePercent = dailyStakingUsdChangePercent;
        ProviderRewardsDailyUsd = providerRewardsDailyUsd;
        MarketRewardsDailyUsd = marketRewardsDailyUsd;
        LiquidityPoolCount = liquidityPoolCount;
    }

    public ulong Id { get; }
    public ulong MarketId { get; }
    public decimal LiquidityUsd { get; private set; }
    public decimal DailyLiquidityUsdChangePercent { get; private set; }
    public decimal VolumeUsd { get; private set; }
    public ulong StakingWeight { get; private set; }
    public decimal DailyStakingWeightChangePercent { get; private set; }
    public decimal StakingUsd { get; private set; }
    public decimal DailyStakingUsdChangePercent { get; private set; }
    public decimal ProviderRewardsDailyUsd { get; private set; }
    public decimal MarketRewardsDailyUsd { get; private set; }
    public ulong LiquidityPoolCount { get; }

    public void Update(MarketSnapshot snapshot, ulong blockHeight)
    {
        LiquidityUsd = snapshot.LiquidityUsd.Close;
        VolumeUsd = snapshot.VolumeUsd;
        StakingWeight = (ulong)snapshot.Staking.Weight.Close;
        StakingUsd = snapshot.Staking.Usd.Close;
        ProviderRewardsDailyUsd = snapshot.Rewards.ProviderUsd;
        MarketRewardsDailyUsd = snapshot.Rewards.MarketUsd;
        DailyLiquidityUsdChangePercent = MathExtensions.PercentChange(snapshot.LiquidityUsd.Close, snapshot.LiquidityUsd.Open);
        DailyStakingUsdChangePercent = MathExtensions.PercentChange(snapshot.Staking.Usd.Close, snapshot.Staking.Usd.Open);
        DailyStakingWeightChangePercent = MathExtensions.PercentChange(snapshot.Staking.Weight.Close, snapshot.Staking.Weight.Open,
                                                                       TokenConstants.Opdex.Sats);
        SetModifiedBlock(blockHeight);
    }
}
