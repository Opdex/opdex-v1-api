using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using System;

namespace Opdex.Platform.Domain.Models.VaultGovernances;

/// <summary>
/// The voting position of an address for a proposal.
/// </summary>
public class VaultProposalVote : BlockAudit
{
    public VaultProposalVote(ulong vaultGovernanceId, ulong proposalId, Address voter, ulong vote, ulong balance, bool inFavor, ulong createdBlock)
        : base(createdBlock)
    {
        VaultGovernanceId = vaultGovernanceId > 0 ? vaultGovernanceId : throw new ArgumentOutOfRangeException(nameof(vaultGovernanceId), "VaultId must be greater than zero.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Voter = voter != Address.Empty ? voter : throw new ArgumentNullException(nameof(voter), "Voter must be provided.");
        Vote = vote > 0 ? vote : throw new ArgumentNullException(nameof(vote), "Vote must be greater than zero.");
        Balance = balance;
        InFavor = inFavor;
    }

    public VaultProposalVote(ulong id, ulong vaultGovernanceId, ulong proposalId, Address voter, ulong vote, ulong balance, bool inFavor,
                             ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        VaultGovernanceId = vaultGovernanceId;
        ProposalId = proposalId;
        Voter = voter;
        Vote = vote;
        Balance = balance;
        InFavor = inFavor;
    }

    public ulong Id { get; }
    public ulong VaultGovernanceId { get; }
    public ulong ProposalId { get; }
    public Address Voter { get; }
    public ulong Vote { get; private set; }
    public ulong Balance { get; private set; }
    public bool InFavor { get; private set; }

    public void Update(VaultProposalVoteSummary summary, ulong blockHeight)
    {
        InFavor = summary.InFavor;
        Balance = summary.Amount;
        SetModifiedBlock(blockHeight);
    }

    public void Update(VaultProposalVoteLog log, ulong blockHeight)
    {
        Vote += log.VoteAmount;
        Balance = log.VoterAmount;
        InFavor = log.InFavor;
        SetModifiedBlock(blockHeight);
    }

    public void Update(VaultProposalWithdrawVoteLog log, ulong blockHeight)
    {
        if (log.VoteWithdrawn) Vote -= log.WithdrawAmount;
        Balance = log.VoterAmount;
        SetModifiedBlock(blockHeight);
    }
}
