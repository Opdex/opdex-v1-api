using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vaults;

public class VaultProposalPledgeEvent : TransactionEvent
{
    public ulong ProposalId { get; set; }
    public Address Pledger { get; set; }
    public FixedDecimal PledgeAmount { get; set; }
    public FixedDecimal PledgerAmount { get; set; }
    public FixedDecimal ProposalPledgeAmount { get; set; }
    public bool TotalPledgeMinimumMet { get; set; }
}
