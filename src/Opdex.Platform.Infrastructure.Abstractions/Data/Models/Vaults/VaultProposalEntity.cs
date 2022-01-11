using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;

public class VaultProposalEntity : AuditEntity
{
    public ulong Id { get; set; }
    public ulong PublicId { get; set; }
    public ulong VaultId { get; set; }
    public Address Creator { get; set; }
    public Address Wallet { get; set; }
    public UInt256 Amount { get; set; }
    public string Description { get; set; }
    public byte ProposalTypeId { get; set; }
    public byte ProposalStatusId { get; set; }
    public ulong Expiration { get; set; }
    public ulong YesAmount { get; set; }
    public ulong NoAmount { get; set; }
    public ulong PledgeAmount { get; set; }
    public bool Approved { get; set; }
}
