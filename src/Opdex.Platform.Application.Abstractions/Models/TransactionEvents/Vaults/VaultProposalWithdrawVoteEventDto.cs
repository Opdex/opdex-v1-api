using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vaults;

public class VaultProposalWithdrawVoteEventDto : TransactionEventDto
{
    public ulong ProposalId { get; set; }
    public Address Voter { get; set; }
    public FixedDecimal WithdrawAmount { get; set; }
    public FixedDecimal VoterAmount { get; set; }
    public FixedDecimal ProposalYesAmount { get; set; }
    public FixedDecimal ProposalNoAmount { get; set; }
    public bool VoteWithdrawn { get; set; }
    public override TransactionEventType EventType => TransactionEventType.VaultProposalWithdrawVoteEvent;
}
