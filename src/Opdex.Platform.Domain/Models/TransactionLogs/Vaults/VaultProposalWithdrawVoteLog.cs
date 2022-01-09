using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

public class VaultProposalWithdrawVoteLog : TransactionLog
{
    public VaultProposalWithdrawVoteLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.VaultProposalWithdrawVoteLog, address, sortOrder)
    {
        ulong proposalId = (ulong)log?.proposalId;
        Address voter = (string)log?.voter;
        ulong withdrawAmount = (ulong)log?.withdrawAmount;
        ulong voterAmount = (ulong)log?.voterAmount;
        ulong proposalYesAmount = (ulong)log?.proposalYesAmount;
        ulong proposalNoAmount = (ulong)log?.proposalNoAmount;
        bool voteWithdrawn = (bool)log?.voteWithdrawn;

        if (proposalId < 1) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than 0.");
        if (voter == Address.Empty) throw new ArgumentNullException(nameof(voter), "Voter must be set.");

        ProposalId = proposalId;
        Voter = voter;
        WithdrawAmount = withdrawAmount;
        VoterAmount = voterAmount;
        ProposalYesAmount = proposalYesAmount;
        ProposalNoAmount = proposalNoAmount;
        VoteWithdrawn = voteWithdrawn;
    }

    public VaultProposalWithdrawVoteLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.VaultProposalWithdrawVoteLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        ProposalId = logDetails.ProposalId;
        Voter = logDetails.Voter;
        WithdrawAmount = logDetails.WithdrawAmount;
        VoterAmount = logDetails.VoterAmount;
        ProposalYesAmount = logDetails.ProposalYesAmount;
        ProposalNoAmount = logDetails.ProposalNoAmount;
        VoteWithdrawn = logDetails.VoteWithdrawn;
    }

    public ulong ProposalId { get; }
    public Address Voter { get; }
    public ulong WithdrawAmount { get; }
    public ulong VoterAmount { get; }
    public ulong ProposalYesAmount { get; }
    public ulong ProposalNoAmount { get; }
    public bool VoteWithdrawn { get; }

    private struct LogDetails
    {
        public ulong ProposalId { get; set; }
        public Address Voter { get; set; }
        public ulong WithdrawAmount { get; set; }
        public ulong VoterAmount { get; set; }
        public ulong ProposalYesAmount { get; set; }
        public ulong ProposalNoAmount { get; set; }
        public bool VoteWithdrawn { get; set; }
    }

    private static LogDetails DeserializeLogDetails(string details)
    {
        return JsonConvert.DeserializeObject<LogDetails>(details);
    }

    public override string SerializeLogDetails()
    {
        return JsonConvert.SerializeObject(new LogDetails
        {
            ProposalId = ProposalId,
            Voter = Voter,
            WithdrawAmount = WithdrawAmount,
            VoterAmount = VoterAmount,
            ProposalYesAmount = ProposalYesAmount,
            ProposalNoAmount = ProposalNoAmount,
            VoteWithdrawn = VoteWithdrawn
        });
    }
}
