using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vaults;

public class CreateVaultProposalEvent : TransactionEvent
{
    public ulong ProposalId { get; set; }
    public Address Wallet { get; set; }
    public FixedDecimal Amount { get; set; }
    public VaultProposalType Type { get; set; }
    public VaultProposalStatus Status { get; set; }
    public ulong Expiration { get; set; }
    public string Description { get; set; }
}
