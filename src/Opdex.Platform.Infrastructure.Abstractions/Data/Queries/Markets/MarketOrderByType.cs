namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

/// <summary>
/// Market sort by filter
/// </summary>
public enum MarketOrderByType
{
    /// <summary>
    /// Orders by the date they were added to Opdex
    /// </summary>
    CreatedBlock = 0,
    LiquidityUsd = 1,
    DailyLiquidityUsdChangePercent = 2,
    VolumeUsd = 3,
    StakingWeight = 4,
    DailyStakingWeightChangePercent = 5,
    StakingUsd = 6,
    DailyStakingUsdChangePercent = 7,
    MarketRewardsDailyUsd = 8,
    ProviderRewardsDailyUsd = 9
}
