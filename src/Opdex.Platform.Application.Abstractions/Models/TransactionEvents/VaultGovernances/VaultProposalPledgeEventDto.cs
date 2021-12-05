using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.VaultGovernances;

public class VaultProposalPledgeEventDto : TransactionEventDto
{
    public ulong ProposalId { get; set; }
    public Address Pledger { get; set; }
    public FixedDecimal PledgeAmount { get; set; }
    public FixedDecimal PledgerAmount { get; set; }
    public FixedDecimal ProposalPledgeAmount { get; set; }
    public bool TotalPledgeMinimumMet { get; set; }
    public override TransactionEventType EventType => TransactionEventType.VaultProposalPledgeEvent;
}
