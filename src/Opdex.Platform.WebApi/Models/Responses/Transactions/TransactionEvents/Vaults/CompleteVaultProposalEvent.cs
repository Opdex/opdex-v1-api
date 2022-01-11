namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vaults;

public class CompleteVaultProposalEvent : TransactionEvent
{
    public ulong ProposalId { get; set; }
    public bool Approved { get; set; }
}
