namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Governances
{
    public class RewardMiningPoolLogDto : TransactionLogDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Amount { get; set; }
    }
}
