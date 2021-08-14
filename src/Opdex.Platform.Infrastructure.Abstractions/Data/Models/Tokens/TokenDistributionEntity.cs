using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX
{
    public class TokenDistributionEntity : AuditEntity
    {
        public long Id { get; set; }
        public string VaultDistribution { get; set; }
        public string MiningGovernanceDistribution { get; set; }
        public int PeriodIndex { get; set; }
        public ulong DistributionBlock { get; set; }
        public ulong NextDistributionBlock { get; set; }
    }
}