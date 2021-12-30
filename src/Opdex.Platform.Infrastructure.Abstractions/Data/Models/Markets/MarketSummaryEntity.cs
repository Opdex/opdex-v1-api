namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Markets;

public class MarketSummaryEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong MarketId { get; set; }
    public decimal LiquidityUsd { get; set; }
    public decimal DailyLiquidityUsdChangePercent { get; set; }
    public decimal VolumeUsd { get; set; }
    public ulong StakingWeight { get; set; }
    public decimal DailyStakingWeightChangePercent { get; set; }
    public decimal StakingUsd { get; set; }
    public decimal DailyStakingUsdChangePercent { get; set; }
    public decimal ProviderRewardsDailyUsd { get; set; }
    public decimal MarketRewardsDailyUsd { get; set; }
}
