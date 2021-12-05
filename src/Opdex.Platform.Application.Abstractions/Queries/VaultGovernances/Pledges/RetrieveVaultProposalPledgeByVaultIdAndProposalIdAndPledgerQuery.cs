using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;

/// <summary>
/// Retrieve a pledger's pledge details from a specific vault proposal.
/// </summary>
public class RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery : FindQuery<VaultProposalPledge>
{
    /// <summary>
    /// Constructor to build a retrieve vault proposal pledge by vault id and proposal id and pledger query.
    /// </summary>
    /// <param name="vaultId">The Id of the vault.</param>
    /// <param name="proposalId">The smart contract's public proposal Id.</param>
    /// <param name="pledger">The pledger's wallet address.</param>
    /// <param name="findOrThrow">Find or throw, defaults to true, when true throws not found exception if no record is found.</param>
    public RetrieveVaultProposalPledgeByVaultIdAndProposalIdAndPledgerQuery(ulong vaultId, ulong proposalId, Address pledger, bool findOrThrow = true)
        : base(findOrThrow)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
        ProposalId = proposalId > 0 ? proposalId : throw new ArgumentOutOfRangeException(nameof(proposalId), "ProposalId must be greater than zero.");
        Pledger = pledger != Address.Empty ? pledger : throw new ArgumentNullException(nameof(pledger), "Pledger address must be provided.");
    }

    public ulong VaultId { get; }
    public ulong ProposalId { get; }
    public Address Pledger { get; }
}
