namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class RewardMiningPoolLogDto : TransactionLogDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Amount { get; set; }
    }
}