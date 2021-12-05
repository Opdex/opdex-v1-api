namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.VaultGovernances;

public class CompleteVaultProposalEvent : TransactionEvent
{
    public ulong ProposalId { get; set; }
    public bool Approved { get; set; }
}
