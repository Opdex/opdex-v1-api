namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens
{
    public class TokenDistributionEntity : AuditEntity
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public string VaultDistribution { get; set; }
        public string MiningGovernanceDistribution { get; set; }
        public int PeriodIndex { get; set; }
        public ulong DistributionBlock { get; set; }
        public ulong NextDistributionBlock { get; set; }
    }
}
