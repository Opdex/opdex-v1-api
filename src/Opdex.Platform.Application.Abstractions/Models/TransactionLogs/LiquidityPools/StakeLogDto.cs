namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.LiquidityPools
{
    public class StakeLogDto : TransactionLogDto
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public string TotalStaked { get; set; }
        public byte EventType { get; set; }
    }
}