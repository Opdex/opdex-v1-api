using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.VaultGovernances;

public class VaultProposalWithdrawVoteEvent : TransactionEvent
{
    public ulong ProposalId { get; set; }
    public Address Voter { get; set; }
    public FixedDecimal WithdrawAmount { get; set; }
    public FixedDecimal VoterAmount { get; set; }
    public FixedDecimal ProposalYesAmount { get; set; }
    public FixedDecimal ProposalNoAmount { get; set; }
    public bool VoteWithdrawn { get; set; }
}
