using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.VaultGovernances;

public class VaultProposalWithdrawPledgeEventDto : TransactionEventDto
{
    public ulong ProposalId { get; set; }
    public Address Pledger { get; set; }
    public FixedDecimal WithdrawAmount { get; set; }
    public FixedDecimal PledgerAmount { get; set; }
    public FixedDecimal ProposalPledgeAmount { get; set; }
    public bool PledgeWithdrawn { get; set; }
    public override TransactionEventType EventType => TransactionEventType.VaultProposalWithdrawPledgeEvent;
}
