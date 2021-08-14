namespace Opdex.Platform.Application.Abstractions.Models.MiningPools
{
    public class MiningPoolDto
    {
        public string Address { get; set; }
        public string LiquidityPool { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
        public string RewardPerBlock { get; set; }
        public string RewardPerLpt { get; set; }
        public string TokensMining { get; set; }
        public bool IsActive { get; set; }
    }
}
