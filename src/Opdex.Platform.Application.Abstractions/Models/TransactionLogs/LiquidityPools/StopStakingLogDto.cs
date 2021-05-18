namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.LiquidityPools
{
    public class StopStakingLogDto : TransactionLogDto
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public string TotalStaked { get; set; }
    }
}