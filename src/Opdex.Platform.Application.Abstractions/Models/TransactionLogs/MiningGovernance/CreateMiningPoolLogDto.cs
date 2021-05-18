namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.MiningGovernance
{
    public class CreateMiningPoolLogDto : TransactionLogDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
    }
}