using Opdex.Platform.Common.Enums;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.VaultGovernances;

public class CompleteVaultProposalEventDto : TransactionEventDto
{
    public ulong ProposalId { get; set; }
    public bool Approved { get; set; }
    public override TransactionEventType EventType => TransactionEventType.CompleteVaultProposalEvent;
}
