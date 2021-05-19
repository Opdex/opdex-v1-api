namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.MiningPools
{
    public class EnableMiningLogDto : TransactionLogDto
    {
        public string Amount { get; set; }
        public string RewardRate { get; set; }
        public ulong MiningPeriodEndBlock { get; set; }
    }
}