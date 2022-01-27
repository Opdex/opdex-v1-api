using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Models.MiningPools;

public class MiningPoolDto
{
    public Address Address { get; set; }
    public Address LiquidityPool { get; set; }
    public ulong MiningPeriodEndBlock { get; set; }
    public UInt256 RewardPerBlock { get; set; }
    public UInt256 RewardPerLpt { get; set; }
    public UInt256 TokensMining { get; set; }
    public bool IsActive { get; set; }
    public ulong CreatedBlock { get; set; }
    public ulong ModifiedBlock { get; set; }
}