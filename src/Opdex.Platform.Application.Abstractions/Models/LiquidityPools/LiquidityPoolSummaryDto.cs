using Opdex.Platform.Application.Abstractions.Models.MiningPools;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools;

public class LiquidityPoolSummaryDto
{
    public VolumeDto Volume { get; set; }
    public RewardsDto Rewards { get; set; }
    public ReservesDto Reserves { get; set; }
    public CostDto Cost { get; set; }
    public StakingDto Staking { get; set; }
    public MiningPoolDto MiningPool { get; set; }
}