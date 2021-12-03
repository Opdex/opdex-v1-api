using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;

public class VaultGovernanceEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong TokenId { get; set; }
    public Address Address { get; set; }
    public UInt256 UnassignedSupply { get; set; }
    public UInt256 ProposedSupply { get; set; }
    public ulong VestingDuration { get; set; }
    public ulong TotalPledgeMinimum { get; set; }
    public ulong TotalVoteMinimum { get; set; }
}
