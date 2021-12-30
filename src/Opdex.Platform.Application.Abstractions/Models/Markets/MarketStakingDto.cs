using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Markets;

public class MarketStakingDto
{
    public FixedDecimal StakingWeight { get; set; }
    public decimal DailyStakingWeightChangePercent { get; set; }
    public decimal StakingUsd { get; set; }
    public decimal DailyStakingUsdChangePercent { get; set; }
}
