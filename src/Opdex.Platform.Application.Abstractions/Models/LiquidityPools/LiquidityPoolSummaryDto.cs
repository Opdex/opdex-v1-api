namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools;

public class LiquidityPoolSummaryDto
{
    public VolumeDto Volume { get; set; }
    public RewardsDto Rewards { get; set; }
    public ReservesDto Reserves { get; set; }
    public CostDto Cost { get; set; }
    public StakingDto Staking { get; set; }
    public ulong CreatedBlock { get; set; }
    public ulong ModifiedBlock { get; set; }
}
