namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs
{
    public class ExitStakingPoolLogDto : TransactionLogDto
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
    }
}