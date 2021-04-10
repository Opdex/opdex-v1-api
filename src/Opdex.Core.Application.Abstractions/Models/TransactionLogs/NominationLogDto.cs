namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class NominationLogDto : TransactionLogDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Weight { get; set; }
    }
}