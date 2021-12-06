using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.VaultGovernances;

public class VaultProposalVoteDto
{
    public Address Vault { get; set; }
    public ulong ProposalId { get; set; }
    public Address Voter { get; set; }
    public FixedDecimal Vote { get; set; }
    public FixedDecimal Balance { get; set; }
    public bool InFavor { get; set; }
}