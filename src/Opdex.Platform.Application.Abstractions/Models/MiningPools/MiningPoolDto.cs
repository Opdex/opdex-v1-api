using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.MiningPools
{
    public class MiningPoolDto
    {
        public Address Address { get; set; }
        public Address LiquidityPool { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
        public string RewardPerBlock { get; set; }
        public string RewardPerLpt { get; set; }
        public string TokensMining { get; set; }
        public bool IsActive { get; set; }
    }
}
