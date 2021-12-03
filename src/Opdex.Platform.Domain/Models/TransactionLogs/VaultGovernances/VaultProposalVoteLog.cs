using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;

public class VaultProposalVoteLog : TransactionLog
{
    public VaultProposalVoteLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.VaultProposalVoteLog, address, sortOrder)
    {
        ulong proposalId = (ulong)log?.proposalId;
        Address voter = (string)log?.voter;
        ulong voteAmount = (ulong)log?.voteAmount;
        ulong voterAmount = (ulong)log?.voterAmount;
        ulong proposalYesAmount = (ulong)log?.proposalYesAmount;
        ulong proposalNoAmount = (ulong)log?.proposalNoAmount;
        bool inFavor = (bool)log?.inFavor;

        if (proposalId < 1) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than 0.");
        if (voter == Address.Empty) throw new ArgumentNullException(nameof(voter), "Voter must be set.");
        if (voteAmount == 0) throw new ArgumentOutOfRangeException(nameof(voteAmount), "Vote amount must be greater than zero.");
        if (voterAmount < voteAmount) throw new ArgumentOutOfRangeException(nameof(voterAmount), "Voter amount must be greater than or equal to vote amount.");

        ProposalId = proposalId;
        Voter = voter;
        VoteAmount = voteAmount;
        VoterAmount = voterAmount;
        ProposalYesAmount = proposalYesAmount;
        ProposalNoAmount = proposalNoAmount;
        InFavor = inFavor;
    }

    public VaultProposalVoteLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.VaultProposalVoteLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        ProposalId = logDetails.ProposalId;
        Voter = logDetails.Voter;
        VoteAmount = logDetails.VoteAmount;
        VoterAmount = logDetails.VoterAmount;
        ProposalYesAmount = logDetails.ProposalYesAmount;
        ProposalNoAmount = logDetails.ProposalNoAmount;
        InFavor = logDetails.InFavor;
    }

    public ulong ProposalId { get; }
    public Address Voter { get; }
    public ulong VoteAmount { get; }
    public ulong VoterAmount { get; }
    public ulong ProposalYesAmount { get; }
    public ulong ProposalNoAmount { get; }
    public bool InFavor { get; }

    private struct LogDetails
    {
        public ulong ProposalId { get; set; }
        public Address Voter { get; set; }
        public ulong VoteAmount { get; set; }
        public ulong VoterAmount { get; set; }
        public ulong ProposalYesAmount { get; set; }
        public ulong ProposalNoAmount { get; set; }
        public bool InFavor { get; set; }
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
            VoteAmount = VoteAmount,
            VoterAmount = VoterAmount,
            ProposalYesAmount = ProposalYesAmount,
            ProposalNoAmount = ProposalNoAmount,
            InFavor = InFavor
        });
    }
}
