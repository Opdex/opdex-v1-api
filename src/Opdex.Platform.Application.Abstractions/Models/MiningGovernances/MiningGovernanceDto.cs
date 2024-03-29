using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.MiningGovernances;

public class MiningGovernanceDto
{
    public Address Address { get; set; }
    public ulong PeriodEndBlock { get; set; }
    public ulong PeriodRemainingBlocks { get; set; }
    public ulong PeriodBlockDuration { get; set; }
    public uint PeriodsUntilRewardReset { get; set; }
    public FixedDecimal MiningPoolRewardPerPeriod { get; set; }
    public FixedDecimal TotalRewardsPerPeriod { get; set; }
    public Address MinedToken { get; set; }
    public ulong CreatedBlock { get; set; }
    public ulong ModifiedBlock { get; set; }
}