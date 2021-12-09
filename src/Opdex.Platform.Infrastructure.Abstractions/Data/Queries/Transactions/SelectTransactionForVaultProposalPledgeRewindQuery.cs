using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;

/// <summary>
/// Selects the latest proposal pledge or pledge withdraw transaction a user has made.
/// </summary>
public class SelectTransactionForVaultProposalPledgeRewindQuery : IRequest<Transaction>
{
    /// <summary>
    /// Constructor to initialize a select transaction for vault proposal pledge rewind query.
    /// </summary>
    /// <param name="vault">The address of the vault.</param>
    /// <param name="pledger">The address of the pledger.</param>
    /// <param name="proposalPublicId">The public Id of the proposal.</param>
    public SelectTransactionForVaultProposalPledgeRewindQuery(Address vault, Address pledger, ulong proposalPublicId)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        Pledger = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Pledger address must be provided.");
        ProposalPublicId = proposalPublicId > 0 ? proposalPublicId : throw new ArgumentOutOfRangeException(nameof(proposalPublicId), "Proposal public Id must be greater than zero.");
    }

    public Address Vault { get; }
    public Address Pledger { get; }
    public ulong ProposalPublicId { get; }
}
