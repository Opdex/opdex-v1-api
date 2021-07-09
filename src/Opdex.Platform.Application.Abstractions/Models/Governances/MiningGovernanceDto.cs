using Opdex.Platform.Application.Abstractions.Models.TokenDtos;

namespace Opdex.Platform.Application.Abstractions.Models.Governances
{
    public class MiningGovernanceDto
    {
        public string Address { get; set; }
        public ulong PeriodEndBlock { get; set; }
        public ulong PeriodRemainingBlocks { get; set; }
        public ulong PeriodBlockDuration { get; set; }
        public uint PeriodsUntilRewardReset { get; set; }
        public string MiningPoolRewardPerPeriod { get; set; }
        public string TotalRewardsPerPeriod { get; set; }
        public TokenDto MinedToken { get; set; }
    }
}
