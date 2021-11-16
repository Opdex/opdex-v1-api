using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.Governances
{
    public class MiningGovernanceResponseModel
    {
        public Address Address { get; set; }

        [Range(1, double.MaxValue)]
        public ulong PeriodEndBlock { get; set; }

        [Range(0, double.MaxValue)]
        public ulong PeriodRemainingBlocks { get; set; }

        [Range(1, double.MaxValue)]
        public ulong PeriodBlockDuration { get; set; }

        [Range(0, double.MaxValue)]
        public uint PeriodsUntilRewardReset { get; set; }

        public FixedDecimal MiningPoolRewardPerPeriod { get; set; }

        public FixedDecimal TotalRewardsPerPeriod { get; set; }
        
        public Address MinedToken { get; set; }
    }
}
