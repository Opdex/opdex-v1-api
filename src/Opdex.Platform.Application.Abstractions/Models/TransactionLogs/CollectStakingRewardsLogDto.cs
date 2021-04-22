namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class CollectStakingRewardsLogDto : TransactionLogDto
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public string Reward { get; set; }
    }
}