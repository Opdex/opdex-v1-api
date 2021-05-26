namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.LiquidityPools
{
    public class CollectStakingRewardsLogDto : TransactionLogDto
    {
        public string Staker { get; set; }
        public string Reward { get; set; }
    }
}