using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Vaults;

public class VaultProposalPledgeDto
{
    public Address Vault { get; set; }
    public ulong ProposalId { get; set; }
    public Address Pledger { get; set; }
    public FixedDecimal Pledge { get; set; }
    public FixedDecimal Balance { get; set; }
}
