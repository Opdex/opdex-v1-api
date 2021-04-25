namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class StartStakingLogDto : TransactionLogDto
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public string TotalStaked { get; set; }
    }
}