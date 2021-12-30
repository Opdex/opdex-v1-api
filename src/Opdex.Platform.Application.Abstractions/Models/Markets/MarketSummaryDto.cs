using Opdex.Platform.Application.Abstractions.Models.LiquidityPools;

namespace Opdex.Platform.Application.Abstractions.Models.Markets;

public class MarketSummaryDto
{
    public ulong Id { get; set; }
    public ulong MarketId { get; set; }
    public decimal LiquidityUsd { get; set; }
    public decimal DailyLiquidityUsdChangePercent { get; set; }
    public decimal VolumeUsd { get; set; }
    public MarketStakingDto Staking { get; set; }
    public RewardsDto Rewards { get; set; }
}
