using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;

public class TokenDistributionEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong TokenId { get; set; }
    public UInt256 VaultDistribution { get; set; }
    public UInt256 MiningGovernanceDistribution { get; set; }
    public int PeriodIndex { get; set; }
    public ulong DistributionBlock { get; set; }
    public ulong NextDistributionBlock { get; set; }
}