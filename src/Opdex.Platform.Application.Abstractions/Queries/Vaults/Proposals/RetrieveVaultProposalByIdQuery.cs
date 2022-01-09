using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;

/// <summary>
/// Retrieve a vault proposal to retrieve its details.
/// </summary>
public class RetrieveVaultProposalByIdQuery : FindQuery<VaultProposal>
{
    /// <summary>
    /// Constructor to build a retrieve vault proposal by id query.
    /// </summary>
    /// <param name="proposalId">The internal proposal Id to lookup the proposal by.</param>
    /// <param name="findOrThrow">Find or throw, defaults to true, when true throws not found exception if no record is found.</param>
    public RetrieveVaultProposalByIdQuery(ulong proposalId, bool findOrThrow = true) : base(findOrThrow)
    {
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
    }

    public ulong ProposalId { get; }
}
