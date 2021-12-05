using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.VaultGovernances;

public class VaultProposalWithdrawPledgeEvent : TransactionEvent
{
    public ulong ProposalId { get; set; }
    public Address Pledger { get; set; }
    public FixedDecimal WithdrawAmount { get; set; }
    public FixedDecimal PledgerAmount { get; set; }
    public FixedDecimal ProposalPledgeAmount { get; set; }
    public bool PledgeWithdrawn { get; set; }
}
