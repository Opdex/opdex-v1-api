using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;

public class VaultProposalVoteEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong VaultGovernanceId { get; set; }
    public ulong ProposalId { get; set; }
    public Address Voter { get; set; }
    public ulong Vote { get; set; }
    public ulong Balance { get; set; }
    public bool InFavor { get; set; }
}
