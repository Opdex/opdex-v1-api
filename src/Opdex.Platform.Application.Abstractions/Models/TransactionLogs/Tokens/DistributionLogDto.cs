namespace Opdex.Platform.Application.Abstractions.Models.TransactionLogs.Tokens
{
    public class DistributionLogDto : TransactionLogDto
    {
        public string VaultAmount { get; set; }
        public string MiningAmount { get; set; }
        public uint PeriodIndex { get; set; }
    }
}