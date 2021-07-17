namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens
{
    public class DistributionEventDto : TransactionEventDto
    {
        public string VaultAmount { get; set; }
        public string MiningAmount { get; set; }
        public uint PeriodIndex { get; set; }
    }
}
