using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.VaultGovernances;

public class CreateVaultProposalEventDto : TransactionEventDto
{
    public ulong ProposalId { get; set; }
    public Address Wallet { get; set; }
    public FixedDecimal Amount { get; set; }
    public VaultProposalType Type { get; set; }
    public VaultProposalStatus Status { get; set; }
    public ulong Expiration { get; set; }
    public string Description { get; set; }
    public override TransactionEventType EventType => TransactionEventType.CreateVaultProposalEvent;
}
