namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.MiningGovernance
{
    public class RewardMiningPoolLogDto : TransactionLogDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Amount { get; set; }
    }
}