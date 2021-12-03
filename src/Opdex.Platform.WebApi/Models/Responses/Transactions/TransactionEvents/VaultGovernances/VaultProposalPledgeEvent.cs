using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.VaultGovernances;

public class VaultProposalPledgeEvent : TransactionEvent
{
    public ulong ProposalId { get; set; }
    public Address Pledger { get; set; }
    public FixedDecimal PledgeAmount { get; set; }
    public FixedDecimal PledgerAmount { get; set; }
    public FixedDecimal ProposalPledgeAmount { get; set; }
    public bool PledgeMinimumMet { get; set; }
}
