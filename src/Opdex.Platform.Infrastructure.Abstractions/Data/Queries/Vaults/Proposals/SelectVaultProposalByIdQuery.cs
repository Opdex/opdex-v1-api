using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;

/// <summary>
/// Select a vault proposal to retrieve it's details by internal Id.
/// </summary>
public class SelectVaultProposalByIdQuery : FindQuery<VaultProposal>
{
    /// <summary>
    /// Constructor to build a select vault proposal by id query.
    /// </summary>
    /// <param name="proposalId">The internal proposal Id to lookup the proposal by.</param>
    /// <param name="findOrThrow">Find or throw, defaults to true, when true throws not found exception if no record is found.</param>
    public SelectVaultProposalByIdQuery(ulong proposalId, bool findOrThrow = true) : base(findOrThrow)
    {
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
    }

    public ulong ProposalId { get; }
}
