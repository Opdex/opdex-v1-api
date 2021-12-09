using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;

/// <summary>
/// Selects the latest proposal vote or vote withdraw transaction a user has made.
/// </summary>
public class SelectTransactionForVaultProposalVoteRewindQuery : IRequest<Transaction>
{
    /// <summary>
    /// Constructor to initialize a select transaction for vault proposal vote rewind query.
    /// </summary>
    /// <param name="vault">The address of the vault.</param>
    /// <param name="voter">The address of the voter.</param>
    /// <param name="proposalPublicId">The public Id of the proposal.</param>
    public SelectTransactionForVaultProposalVoteRewindQuery(Address vault, Address voter, ulong proposalPublicId)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        Voter = voter != Address.Empty ? voter : throw new ArgumentNullException(nameof(voter), "Voter address must be provided.");
        ProposalPublicId = proposalPublicId > 0 ? proposalPublicId : throw new ArgumentOutOfRangeException(nameof(proposalPublicId), "Proposal public Id must be greater than zero.");
    }

    public Address Vault { get; }
    public Address Voter { get; }
    public ulong ProposalPublicId { get; }
}
