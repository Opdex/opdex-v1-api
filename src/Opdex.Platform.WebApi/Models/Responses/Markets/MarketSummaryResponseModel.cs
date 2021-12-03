using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

namespace Opdex.Platform.WebApi.Models.Responses.Markets;

public class MarketSummaryResponseModel
{
    public decimal LiquidityUsd { get; set; }
    public decimal DailyLiquidityUsdChangePercent { get; set; }
    public decimal VolumeUsd { get; set; }
    public FixedDecimal StakingWeight { get; set; }
    public decimal DailyStakingWeightChangePercent { get; set; }
    public decimal StakingUsd { get; set; }
    public decimal DailyStakingUsdChangePercent { get; set; }
    public RewardsResponseModel Rewards { get; set; }
}