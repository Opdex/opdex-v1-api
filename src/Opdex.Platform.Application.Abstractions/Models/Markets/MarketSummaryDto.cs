using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Markets;

public class MarketSummaryDto
{
    public decimal LiquidityUsd { get; set; }
    public decimal DailyLiquidityUsdChangePercent { get; set; }
    public decimal VolumeUsd { get; set; }
    public FixedDecimal StakingWeight { get; set; }
    public decimal DailyStakingWeightChangePercent { get; set; }
    public decimal StakingUsd { get; set; }
    public decimal DailyStakingUsdChangePercent { get; set; }
    public RewardsDto Rewards { get; set; }
}