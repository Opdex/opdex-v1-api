namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Governances
{
    public class NominationLogDto : TransactionLogDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
        public string Weight { get; set; }
    }
}
