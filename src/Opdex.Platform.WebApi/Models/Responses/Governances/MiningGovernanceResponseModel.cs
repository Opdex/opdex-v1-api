using System;

namespace Opdex.Platform.WebApi.Models.Responses.Governances
{
    public class MiningGovernanceResponseModel
    {
        public string Address { get; set; }
        public ulong NominationPeriodEndBlock { get; set; }
        public ulong NominationDuration { get; set; }
        public uint NominationPeriodReset { get; set; }
        public string MiningPoolReward { get; set; }
        public string TotalRewardPerPeriod { get; set; }
        public TokenResponseModel MinedToken { get; set; }
    }
}
