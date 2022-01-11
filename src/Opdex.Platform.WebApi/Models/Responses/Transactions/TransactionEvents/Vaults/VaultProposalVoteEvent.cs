using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Vaults;

public class VaultProposalVoteEvent : TransactionEvent
{
    public ulong ProposalId { get; set; }
    public Address Voter { get; set; }
    public FixedDecimal VoteAmount { get; set; }
    public FixedDecimal VoterAmount { get; set; }
    public FixedDecimal ProposalYesAmount { get; set; }
    public FixedDecimal ProposalNoAmount { get; set; }
    public bool InFavor { get; set; }
}
