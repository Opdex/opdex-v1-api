namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens
{
    public class TokenDistributionEntity
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public long MiningGovernanceId { get; set; }
        public string Owner { get; set; }
        public ulong Genesis { get; set; }
        public ulong PeriodDuration { get; set; }
        public int PeriodIndex { get; set; }
    }
}