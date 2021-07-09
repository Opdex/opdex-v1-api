namespace Opdex.Platform.WebApi.Models.Responses.Governances
{
    public class MiningGovernanceResponseModel
    {
        public string Address { get; set; }
        public ulong PeriodEndBlock { get; set; }
        public ulong PeriodRemainingBlocks { get; set; }
        public ulong PeriodBlockDuration { get; set; }
        public uint PeriodsUntilRewardReset { get; set; }
        public string MiningPoolRewardPerPeriod { get; set; }
        public string TotalRewardsPerPeriod { get; set; }
        public TokenResponseModel MinedToken { get; set; }
    }
}
