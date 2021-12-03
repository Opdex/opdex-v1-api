using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;

public class LiquidityPoolSummaryEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong LiquidityPoolId { get; set; }
    public decimal LiquidityUsd { get; set; }
    public decimal DailyLiquidityUsdChangePercent { get; set; }
    public decimal VolumeUsd { get; set; }
    public ulong StakingWeight { get; set; }
    public decimal DailyStakingWeightChangePercent { get; set; }
    public ulong LockedCrs { get; set; }
    public UInt256 LockedSrc { get; set; }
}