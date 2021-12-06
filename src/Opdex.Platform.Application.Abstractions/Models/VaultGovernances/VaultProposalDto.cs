using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;

namespace Opdex.Platform.Application.Abstractions.Models.VaultGovernances;

public class VaultProposalDto
{
    public Address Vault { get; set; }
    public Address Token { get; set; }
    public ulong ProposalId { get; set; }
    public Address Creator { get; set; }
    public Address Wallet { get; set; }
    public FixedDecimal Amount { get; set; }
    public string Description { get; set; }
    public VaultProposalType Type { get; set; }
    public VaultProposalStatus Status { get; set; }
    public ulong Expiration { get; set; }
    public FixedDecimal YesAmount { get; set; }
    public FixedDecimal NoAmount { get; set; }
    public FixedDecimal PledgeAmount { get; set; }
    public bool Approved { get; set; }
}