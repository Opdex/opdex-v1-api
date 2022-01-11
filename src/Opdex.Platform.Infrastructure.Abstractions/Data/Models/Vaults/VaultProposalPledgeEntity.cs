using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;

public class VaultProposalPledgeEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong VaultId { get; set; }
    public ulong ProposalId { get; set; }
    public Address Pledger { get; set; }
    public ulong Pledge { get; set; }
    public ulong Balance { get; set; }
}
