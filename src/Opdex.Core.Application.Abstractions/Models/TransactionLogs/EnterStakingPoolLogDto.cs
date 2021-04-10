namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class EnterStakingPoolLogDto : TransactionLogDto
    {
        public string Staker { get; set; }
        public string Amount { get; set; }
        public string Weight { get; set; }
    }
}