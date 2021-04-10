namespace Opdex.Core.Application.Abstractions.Models.TransactionLogs
{
    public class DistributionLogDto : TransactionLogDto
    {
        public string OwnerAddress { get; set; }
        public string MiningAddress { get; set; }
        public string OwnerAmount { get; set; }
        public string MiningAmount { get; set; }
        public uint PeriodIndex { get; set; }
    }
}