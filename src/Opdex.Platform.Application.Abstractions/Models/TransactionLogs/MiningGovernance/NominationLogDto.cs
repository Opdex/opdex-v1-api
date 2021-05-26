namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.MiningGovernance
{
    public class NominationLogDto : TransactionLogDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Weight { get; set; }
    }
}