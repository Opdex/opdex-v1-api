namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class MiningPoolCreatedLogDto : TransactionLogDto
    {
        public string StakingPool { get; set; }
        public string MiningPool { get; set; }
    }
}