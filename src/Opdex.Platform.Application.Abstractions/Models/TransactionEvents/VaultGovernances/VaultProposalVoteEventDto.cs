using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.VaultGovernances;

public class VaultProposalVoteEventDto : TransactionEventDto
{
    public ulong ProposalId { get; set; }
    public Address Voter { get; set; }
    public FixedDecimal VoteAmount { get; set; }
    public FixedDecimal VoterAmount { get; set; }
    public FixedDecimal ProposalYesAmount { get; set; }
    public FixedDecimal ProposalNoAmount { get; set; }
    public bool InFavor { get; set; }
    public override TransactionEventType EventType => TransactionEventType.VaultProposalVoteEvent;
}
